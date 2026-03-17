# Roadmap: Medical Practice Scheduler Automation Demo

## Overview

This milestone turns the existing scheduling demo into a broader medical practice automation story. The roadmap starts with a polished smart scheduler, then layers in eligibility, intake, referrals and authorizations, billing readiness, operational task routing, and proactive recall workflows so the product tells a credible end-to-end automation narrative in the interview.

## Phases

- [ ] **Phase 1: Smart Scheduling Foundation** - Convert the scheduler into the primary calendar-driven workflow with conflict-aware booking.
- [ ] **Phase 2: Eligibility Verification Workflow** - Add eligibility status, review actions, and visit-level readiness cues.
- [ ] **Phase 3: Patient Intake and Digital Forms** - Track intake completion and pre-visit data from the staff workflow.
- [ ] **Phase 4: Prior Auth and Referral Tracking** - Surface appointment prerequisites that block care or billing.
- [ ] **Phase 5: Claim Scrubbing and Charge Readiness** - Flag billing issues before a visit or charge is marked ready.
- [ ] **Phase 6: Inbox and Task Routing Automation** - Centralize follow-up work into a shared queue with routing and statuses.
- [ ] **Phase 7: Follow-Up and Recall Automation** - Show how overdue patients and recall work can feed back into scheduling.

## Phase Details

### Phase 1: Smart Scheduling Foundation
**Goal**: Turn the existing scheduler into an interview-ready scheduling workspace with conflict-aware booking and a usable calendar flow.
**Depends on**: Nothing beyond the current patient, provider, and appointment baseline
**Requirements**: SCHED-01, SCHED-02, SCHED-03, SCHED-04, SCHED-05
**Success Criteria** (what must be TRUE):
  1. User can schedule appointments by selecting patient and provider and clicking calendar time slots.
  2. Provider conflicts are prevented and surfaced clearly in the UI.
  3. Calendar supports day, week, and month views with visible appointment blocks.
  4. Appointments are stored and reloaded from the ASP.NET API.
**Plans**: 2 plans

Plans:
- [ ] 01-01: Refine appointment scheduler UI, slot interaction, and calendar navigation
- [ ] 01-02: Harden appointment API wiring, conflict handling, and scheduler state refresh

### Phase 2: Eligibility Verification Workflow
**Goal**: Add insurance eligibility awareness directly into scheduling and patient workflows.
**Depends on**: Phase 1
**Requirements**: ELIG-01, ELIG-02, ELIG-03, ELIG-04
**Success Criteria** (what must be TRUE):
  1. User can see eligibility status for a patient before or during appointment scheduling.
  2. Appointments display a clear eligibility state such as verified, pending, or failed.
  3. Staff can trigger or simulate an eligibility check from the UI.
  4. Eligibility issues create visible follow-up tasks or warnings.
**Plans**: 2 plans

Plans:
- [ ] 02-01: Add eligibility models, API endpoints, and patient-level eligibility fields
- [ ] 02-02: Surface eligibility states and review actions inside scheduler and patient flows

### Phase 3: Patient Intake and Digital Forms
**Goal**: Automate pre-visit intake so staff do less manual data entry before appointments.
**Depends on**: Phase 2
**Requirements**: INTAKE-01, INTAKE-02, INTAKE-03, INTAKE-04
**Success Criteria** (what must be TRUE):
  1. User can create and manage intake packets or intake status tied to upcoming appointments.
  2. Patient records show intake completion state and missing items.
  3. Staff can view submitted demographic and form data inside the app.
  4. Incomplete intake is surfaced before check-in or scheduling confirmation.
**Plans**: 3 plans

Plans:
- [ ] 03-00: Add Wave 0 Angular and .NET test infrastructure for intake workflows
- [ ] 03-01: Add backend patient intake contracts, completeness rules, and appointment readiness projection
- [ ] 03-02: Build Angular intake management UI and scheduler readiness indicators

### Phase 4: Prior Auth and Referral Tracking
**Goal**: Track visit prerequisites that block care or billing, especially referrals and authorizations.
**Depends on**: Phase 2
**Requirements**: AUTH-01, AUTH-02, AUTH-03, AUTH-04
**Success Criteria** (what must be TRUE):
  1. Staff can create referral and authorization records linked to patients and appointments.
  2. Each appointment shows whether authorization or referral is required and its current status.
  3. Expired, missing, or pending items are highlighted before the visit.
  4. Staff can move items through statuses such as needed, submitted, approved, or denied.
**Plans**: 3 plans

Plans:
- [ ] 04-00: Add Wave 0 backend and scheduler regression coverage for auth/referral workflows
- [ ] 04-01: Add referral and authorization backend models, status rules, appointment summaries, and linked endpoints
- [ ] 04-02: Add scheduling-side auth and referral views, alerts, follow-up details, and update actions

### Phase 5: Claim Scrubbing and Charge Readiness
**Goal**: Add pre-billing checks that catch obvious claim issues before submission.
**Depends on**: Phase 4
**Requirements**: CLAIM-01, CLAIM-02, CLAIM-03, CLAIM-04
**Success Criteria** (what must be TRUE):
  1. Appointments or visits can be marked billing-ready only when required data is present.
  2. The app flags common claim issues such as missing eligibility, missing auth, or incomplete intake.
  3. Staff can review a simple scrub results panel with actionable warnings.
  4. Billing readiness is visible in the schedule or visit workflow.
**Plans**: 2 plans

Plans:
- [ ] 05-01: Add charge-ready data model, scrub rule evaluation, and billing status endpoints
- [ ] 05-02: Surface scrub warnings and billing readiness across schedule and detail views

### Phase 6: Inbox and Task Routing Automation
**Goal**: Centralize operational follow-up into a lightweight work queue for staff.
**Depends on**: Phases 3, 4, and 5
**Requirements**: TASK-01, TASK-02, TASK-03, TASK-04
**Success Criteria** (what must be TRUE):
  1. Eligibility failures, auth gaps, intake gaps, and billing issues generate tasks automatically.
  2. Tasks are grouped into a shared inbox with status, owner, and priority.
  3. Staff can filter tasks by category such as scheduling, eligibility, auth, or billing.
  4. Resolving the underlying issue updates or closes the related task.
**Plans**: 3 plans

Plans:
- [ ] 06-01: Add task queue backend models, routing buckets, and generation rules
- [ ] 06-02: Build shared inbox UI with filters, status changes, and linked record navigation
- [ ] 06-03: Wire automation events from eligibility, intake, auth, and claim workflows into task creation

### Phase 7: Follow-Up and Recall Automation
**Goal**: Show proactive patient outreach value by identifying who should be brought back into care.
**Depends on**: Phases 1 and 6
**Requirements**: RECALL-01, RECALL-02, RECALL-03, RECALL-04
**Success Criteria** (what must be TRUE):
  1. The app identifies patients due for follow-up, recall, or recurring visits.
  2. Staff can view a recall list with reason, due date, and assigned provider.
  3. Recall candidates can be converted into scheduled appointments from the same workflow.
  4. The dashboard shows upcoming recalls and unresolved follow-up opportunities.
**Plans**: 2 plans

Plans:
- [ ] 07-01: Add recall models, due logic, and appointment conversion endpoints
- [ ] 07-02: Build recall dashboard and scheduling conversion workflow in Angular

## Progress

**Execution Order:**
Phases execute in numeric order: 1 → 2 → 3 → 4 → 5 → 6 → 7

| Phase | Plans Complete | Status | Completed |
|-------|----------------|--------|-----------|
| 1. Smart Scheduling Foundation | 0/2 | Not started | - |
| 2. Eligibility Verification Workflow | 0/2 | Not started | - |
| 3. Patient Intake and Digital Forms | 0/2 | Not started | - |
| 4. Prior Auth and Referral Tracking | 0/2 | Not started | - |
| 5. Claim Scrubbing and Charge Readiness | 0/2 | Not started | - |
| 6. Inbox and Task Routing Automation | 0/3 | Not started | - |
| 7. Follow-Up and Recall Automation | 0/2 | Not started | - |
