# Requirements: Medical Practice Scheduler Automation Demo

**Defined:** 2026-03-15
**Core Value:** Front-desk and back-office staff can manage scheduling and operational follow-up with less manual coordination.

## v1 Requirements

### Scheduling Automation

- [ ] **SCHED-01**: Staff can create appointments from a calendar view by selecting patient, provider, and an available time slot.
- [ ] **SCHED-02**: The system prevents provider double-booking and shows a clear conflict message.
- [ ] **SCHED-03**: The scheduler supports day, week, and month views for appointment management.
- [ ] **SCHED-04**: The system highlights open slots and upcoming appointments for selected providers.
- [ ] **SCHED-05**: The system captures a default scheduled appointment reason and duration for quick booking.

### Eligibility and Coverage

- [ ] **ELIG-01**: Staff can view an eligibility status badge for each upcoming appointment.
- [ ] **ELIG-02**: The system can mark appointments as verified, pending, or failed eligibility.
- [ ] **ELIG-03**: Staff can record payer name, member ID, and verification timestamp for a patient.
- [ ] **ELIG-04**: The appointment list flags visits that still require eligibility review before the visit date.

### Prior Authorization and Referral Tracking

- [x] **AUTH-01**: Staff can create an authorization or referral record linked to a patient and appointment.
- [x] **AUTH-02**: Each authorization/referral record tracks status such as needed, submitted, approved, denied, or expired.
- [x] **AUTH-03**: The system shows missing or unresolved authorizations on the scheduling workflow.
- [x] **AUTH-04**: Staff can record due dates and notes for authorization follow-up.

### Charge Capture and Claim Scrubbing

- [ ] **CLAIM-01**: Staff can record basic visit charge details linked to an appointment.
- [ ] **CLAIM-02**: The system flags missing claim-critical fields before a charge is marked ready.
- [ ] **CLAIM-03**: The system surfaces simple scrub warnings such as missing diagnosis, missing provider, or incomplete insurance data.
- [ ] **CLAIM-04**: Staff can view charge status as draft, review needed, or ready to submit.

### Patient Intake and Forms

- [x] **INTAKE-01**: Staff can create and update a patient demographic record from the Angular UI.
- [x] **INTAKE-02**: The system stores intake status for each patient as not started, in progress, or complete.
- [x] **INTAKE-03**: Staff can record core intake fields such as date of birth, contact info, insurance summary, and intake notes.
- [x] **INTAKE-04**: Upcoming appointments indicate whether patient intake is complete.

### Inbox and Task Routing

- [ ] **TASK-01**: The system provides a shared task queue for scheduling, insurance, auth, billing, and follow-up work items.
- [ ] **TASK-02**: Each task has a type, status, due date, linked patient, and linked appointment when applicable.
- [ ] **TASK-03**: The system can route tasks to a work bucket such as Front Desk, Eligibility, Referrals, Billing, or Clinical Follow-Up.
- [ ] **TASK-04**: Staff can mark tasks as open, in progress, blocked, or complete.

### Follow-Up and Recall Automation

- [ ] **RECALL-01**: The system can create recall items for patients due for follow-up visits.
- [ ] **RECALL-02**: Recall items track reason, due date, status, and assigned work bucket.
- [ ] **RECALL-03**: Staff can convert a recall item into a scheduled appointment.
- [ ] **RECALL-04**: The dashboard surfaces overdue and upcoming recall work.

## v2 Requirements

### Scheduling Automation

- **SCHED-06**: The system recommends appointment length by visit type.
- **SCHED-07**: The system offers waitlist fill suggestions for cancelled slots.
- **SCHED-08**: The system sends reminder workflows automatically by appointment status.

### Eligibility and Coverage

- **ELIG-05**: The system performs real-time payer API eligibility checks.
- **ELIG-06**: The system auto-creates tasks from failed eligibility responses.

### Prior Authorization and Referral Tracking

- **AUTH-05**: The system stores attachment requirements and submission channels by payer.
- **AUTH-06**: The system notifies staff before authorization expiration.

### Charge Capture and Claim Scrubbing

- **CLAIM-05**: The system suggests scrub fixes based on payer-specific rules.
- **CLAIM-06**: The system exports clean claims to an external clearinghouse.

### Patient Intake and Forms

- **INTAKE-05**: Patients can complete intake forms through a self-service portal.
- **INTAKE-06**: Intake forms support digital signatures and document upload.

### Inbox and Task Routing

- **TASK-05**: The system auto-generates tasks from appointment, eligibility, and auth events.
- **TASK-06**: The system prioritizes tasks by urgency and due date.

### Follow-Up and Recall Automation

- **RECALL-05**: The system suggests recall intervals by diagnosis or visit type.
- **RECALL-06**: The system automates outreach tracking for calls, texts, and portal messages.

## Out of Scope

| Feature | Reason |
|---------|--------|
| Full production EHR charting | Too broad for an interview demo and not needed to show operational automation |
| Real payer API integrations | Better represented with demo statuses and workflow states in v1 |
| Actual claim submission to clearinghouses | External integration complexity is too high for current scope |
| HIPAA-grade security/compliance implementation | Important in production, but outside interview-demo scope |
| Multi-location enterprise permissions | Adds admin complexity without improving the core demo story |
| Patient portal authentication and messaging | Better deferred until core staff workflows are complete |
| ML-based predictive scheduling or denial prediction | Strong future story, but unnecessary for a practical v1 demo |

## Traceability

| Requirement | Phase | Status |
|-------------|-------|--------|
| SCHED-01 | Phase 1 | Pending |
| SCHED-02 | Phase 1 | Pending |
| SCHED-03 | Phase 1 | Pending |
| SCHED-04 | Phase 1 | Pending |
| SCHED-05 | Phase 1 | Pending |
| ELIG-01 | Phase 2 | Pending |
| ELIG-02 | Phase 2 | Pending |
| ELIG-03 | Phase 2 | Pending |
| ELIG-04 | Phase 2 | Pending |
| INTAKE-01 | Phase 3 | Complete |
| INTAKE-02 | Phase 3 | Complete |
| INTAKE-03 | Phase 3 | Complete |
| INTAKE-04 | Phase 3 | Complete |
| AUTH-01 | Phase 4 | Complete |
| AUTH-02 | Phase 4 | Complete |
| AUTH-03 | Phase 4 | Complete |
| AUTH-04 | Phase 4 | Complete |
| CLAIM-01 | Phase 5 | Pending |
| CLAIM-02 | Phase 5 | Pending |
| CLAIM-03 | Phase 5 | Pending |
| CLAIM-04 | Phase 5 | Pending |
| TASK-01 | Phase 6 | Pending |
| TASK-02 | Phase 6 | Pending |
| TASK-03 | Phase 6 | Pending |
| TASK-04 | Phase 6 | Pending |
| RECALL-01 | Phase 7 | Pending |
| RECALL-02 | Phase 7 | Pending |
| RECALL-03 | Phase 7 | Pending |
| RECALL-04 | Phase 7 | Pending |

**Coverage:**
- v1 requirements: 29 total
- Mapped to phases: 29
- Unmapped: 0 ✓

---
*Requirements defined: 2026-03-15*
*Last updated: 2026-03-15 after initial roadmap creation*
