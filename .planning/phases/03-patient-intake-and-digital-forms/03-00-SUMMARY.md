---
phase: 03-patient-intake-and-digital-forms
plan: 00
subsystem: testing
tags: [angular, karma, chromeheadless, xunit, dotnet, patient-intake, scheduler]
requires:
  - phase: 01-smart-scheduling-foundation
    provides: existing Angular scheduler components and in-memory scheduling services used by the new test coverage
provides:
  - Headless Angular CLI test entry point for standalone intake and scheduler components
  - Backend xUnit project covering patient normalization and appointment readiness behavior
  - Regression specs aligned with the current nested intake form and expanded intake DTO shapes
affects: [03-patient-intake-and-digital-forms, validation, testing, api, ui]
tech-stack:
  added: [karma, jasmine, xunit, Microsoft.NET.Test.Sdk]
  patterns: [Angular standalone component specs with service spies, in-memory .NET service tests]
key-files:
  created: [tsconfig.spec.json, src/app/patient-form.component.spec.ts, src/app/appointment-scheduler.component.spec.ts, MedicalDemo.Api.Tests/PatientServiceTests.cs, MedicalDemo.Api.Tests/AppointmentServiceTests.cs]
  modified: [package.json, package-lock.json, angular.json, MedicalDemo.Api.Tests/MedicalDemo.Api.Tests.csproj]
key-decisions:
  - "Use Angular CLI Karma/Jasmine test wiring instead of introducing a different frontend test framework."
  - "Keep backend service coverage in a dedicated xUnit project that references the API and exercises in-memory repositories."
  - "Align new specs to the current nested intake form and expanded DTOs already present in the workspace rather than reverting production code to the older plan snapshot."
patterns-established:
  - "Frontend validation foundation: headless Chrome runs standalone component specs from repo root."
  - "Backend validation foundation: xUnit tests verify service behavior through in-memory repositories and NullLogger."
requirements-completed: [INTAKE-01, INTAKE-02, INTAKE-03, INTAKE-04]
duration: 15min
completed: 2026-03-16
---

# Phase 3 Plan 00: Patient Intake Validation Foundation Summary

**Headless Angular intake tests and an xUnit backend service harness now verify patient form flows, scheduler readiness warnings, and intake-related appointment behavior.**

## Performance

- **Duration:** 15 min
- **Started:** 2026-03-16T16:08:00-06:00
- **Completed:** 2026-03-16T16:23:24-06:00
- **Tasks:** 3
- **Files modified:** 9

## Accomplishments

- Added runnable Angular test infrastructure with a root `npm test` command and ChromeHeadless execution.
- Added patient form and scheduler specs covering create/update guards plus readiness warning behavior.
- Added a backend xUnit project for patient and appointment service regression coverage.

## Task Commits

1. **Task 1: Add Angular test runner wiring and patient intake form specs** - `56eb7f3` (`test`)
2. **Task 2: Add backend intake service test project and rule coverage** - `d240645` (`test`)
3. **Task 3: Add scheduler readiness specs for intake badges and warnings** - `8f30df1` (`test`)

**Blocking fix:** `1958b10` (`fix`) stabilized `MedicalDemo.Api.Tests.csproj` so clean `dotnet test` runs resolve xUnit correctly.

## Files Created/Modified

- `package.json` - Added the root Angular headless test script and frontend test dependencies.
- `package-lock.json` - Captured installed Angular/Karma dependency changes.
- `angular.json` - Added the Angular CLI `test` target for headless component specs.
- `tsconfig.spec.json` - Added the TypeScript config for Angular spec compilation.
- `src/app/patient-form.component.spec.ts` - Added create, edit, and invalid-save coverage for the nested patient intake form.
- `src/app/appointment-scheduler.component.spec.ts` - Added readiness-state and warning coverage for scheduler appointments.
- `MedicalDemo.Api.Tests/MedicalDemo.Api.Tests.csproj` - Added and then stabilized the backend xUnit test project metadata.
- `MedicalDemo.Api.Tests/PatientServiceTests.cs` - Added patient service normalization and intake-related regression tests.
- `MedicalDemo.Api.Tests/AppointmentServiceTests.cs` - Added appointment readiness projection and eligibility propagation tests.

## Decisions Made

- Used the Angular CLI/Karma path already implied by the repo instead of adding Vitest or another frontend runner.
- Verified scheduler readiness in component logic plus a week-slot DOM warning check because the current UI exposes warning copy there most reliably.
- Left unrelated production Phase 3 edits untouched and adapted the new tests to the workspace’s in-progress intake model.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Aligned Angular specs to the current intake form and DTO shape**
- **Found during:** Task 3
- **Issue:** The workspace had already evolved from the plan snapshot to a nested patient form plus expanded intake DTOs, which broke the first-pass specs.
- **Fix:** Reworked the patient and scheduler specs to use the current nested form model and expanded appointment/patient types.
- **Files modified:** `src/app/patient-form.component.spec.ts`, `src/app/appointment-scheduler.component.spec.ts`
- **Verification:** `npm test -- --watch=false --browsers=ChromeHeadless`
- **Committed in:** `8f30df1`

**2. [Rule 3 - Blocking] Normalized backend test project metadata for clean xUnit runs**
- **Found during:** Final verification
- **Issue:** `dotnet test` intermittently failed to resolve xUnit attributes in a clean verification pass.
- **Fix:** Updated `MedicalDemo.Api.Tests.csproj` to a stable SDK test-project configuration with `IsTestProject` and runner asset metadata.
- **Files modified:** `MedicalDemo.Api.Tests/MedicalDemo.Api.Tests.csproj`
- **Verification:** `dotnet test MedicalDemo.Api.Tests/MedicalDemo.Api.Tests.csproj`
- **Committed in:** `1958b10`

---

**Total deviations:** 2 auto-fixed (2 Rule 3 blocking issues)
**Impact on plan:** Both fixes were required to keep the planned verification runnable against the current workspace. No scope expansion beyond test foundation work.

## Issues Encountered

- ChromeHeadless could not launch inside the sandbox, so Angular verification was run outside the sandbox.
- Initial backend restore/test also required out-of-sandbox execution because NuGet access was blocked in the sandbox.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- Wave 0 validation is in place for the intake UI and backend service layer.
- Later Phase 3 implementation can build against executable Angular and .NET regression coverage instead of placeholder commands.

## Self-Check: PASSED

---
*Phase: 03-patient-intake-and-digital-forms*
*Completed: 2026-03-16*
