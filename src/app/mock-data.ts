export interface Patient {
  id: string;
  name: string;
  age: number;
  payer: string;
  balance: string;
  lastVisit: string;
  appointment: string;
  reason: string;
  status: string;
  room: string;
  risk: "Low" | "Moderate" | "High";
}

export const alerts = [
  "12 claims missing modifiers",
  "3 charts unsigned from yesterday",
  "Eligibility queue has 7 unresolved visits",
  "2 refill requests waiting more than 24h",
];

export const pmMetrics = [
  { label: "Collections", value: "$42.8K", delta: "+6.4%" },
  { label: "No-shows", value: "3.1%", delta: "-0.8%" },
  { label: "Days in A/R", value: "28", delta: "-2 days" },
  { label: "Claims Ready", value: "46", delta: "11 new" },
];

export const ehrMetrics = [
  { label: "Unsigned notes", value: "5", delta: "2 urgent" },
  { label: "Open encounters", value: "14", delta: "4 roomed" },
  { label: "Lab inbox", value: "19", delta: "6 critical" },
  { label: "Refill requests", value: "8", delta: "2 overdue" },
];

export const patients: Patient[] = [
  {
    id: "PT-10483",
    name: "Olivia Bennett",
    age: 42,
    payer: "Aetna PPO",
    balance: "$184.22",
    lastVisit: "Mar 11",
    appointment: "09:30 AM",
    reason: "Diabetes follow-up",
    status: "Checked in",
    room: "Exam 3",
    risk: "Moderate",
  },
  {
    id: "PT-10477",
    name: "Marcus Hill",
    age: 57,
    payer: "Medicare",
    balance: "$0.00",
    lastVisit: "Feb 27",
    appointment: "10:15 AM",
    reason: "Hypertension review",
    status: "Roomed",
    room: "Exam 5",
    risk: "Low",
  },
  {
    id: "PT-10452",
    name: "Camila Nguyen",
    age: 31,
    payer: "BCBS",
    balance: "$63.40",
    lastVisit: "Jan 18",
    appointment: "11:00 AM",
    reason: "Annual preventive",
    status: "Needs eligibility",
    room: "Front desk",
    risk: "Low",
  },
  {
    id: "PT-10411",
    name: "David Ortiz",
    age: 64,
    payer: "UHC Advantage",
    balance: "$412.98",
    lastVisit: "Mar 01",
    appointment: "01:15 PM",
    reason: "Post-op evaluation",
    status: "Provider ready",
    room: "Exam 1",
    risk: "High",
  },
];

export const schedule = [
  {
    time: "09:30",
    patient: "Olivia Bennett",
    provider: "Dr. Shah",
    type: "Follow-up",
    lane: "In office",
  },
  {
    time: "10:15",
    patient: "Marcus Hill",
    provider: "Dr. Shah",
    type: "Chronic care",
    lane: "In office",
  },
  {
    time: "11:00",
    patient: "Camila Nguyen",
    provider: "Dr. Nelson",
    type: "Physical",
    lane: "In office",
  },
  {
    time: "13:15",
    patient: "David Ortiz",
    provider: "Dr. Nelson",
    type: "Surgery follow-up",
    lane: "Telehealth",
  },
];

export const claimQueue = [
  { claim: "CLM-88412", patient: "Olivia Bennett", payer: "Aetna PPO", amount: "$184.22", status: "Ready to submit" },
  { claim: "CLM-88405", patient: "Marcus Hill", payer: "Medicare", amount: "$126.00", status: "Need modifier" },
  { claim: "CLM-88391", patient: "Camila Nguyen", payer: "BCBS", amount: "$221.40", status: "Hold for eligibility" },
  { claim: "CLM-88374", patient: "David Ortiz", payer: "UHC Advantage", amount: "$612.00", status: "Coding review" },
];

export const masterFiles = [
  { name: "Providers", detail: "18 active rendering providers and 4 supervising providers." },
  { name: "Locations", detail: "3 clinics, 1 ASC, and 2 telehealth service addresses." },
  { name: "Fee Schedules", detail: "Commercial, Medicare, self-pay, and workers comp schedules." },
  { name: "Payers", detail: "87 active payer profiles with electronic claim routing." },
];

export const userSettings = [
  { name: "Roles & Permissions", detail: "Configure access by role, specialty, and operational team." },
  { name: "Inbox Preferences", detail: "Control task routing, refill pools, and notification thresholds." },
  { name: "Scheduling Defaults", detail: "Set personal views, appointment colors, and provider filters." },
  { name: "Favorites & Macros", detail: "Manage saved phrases, favorite orders, and quick actions." },
];

export const reports = [
  { name: "Daily Collections", detail: "Cash, card, ERA, and adjustment totals by location." },
  { name: "Appointment Utilization", detail: "Provider fill rate, no-show trend, and wait time." },
  { name: "A/R Aging", detail: "Bucketed aging by payer, location, and financial class." },
  { name: "Productivity", detail: "Signed encounters, charges posted, and claims submitted." },
];

export const chartTabs = ["Summary", "HPI", "Orders", "Labs", "Billing"];

export const timeline = [
  {
    label: "Vitals",
    detail: "BP 138/84, HR 76, A1C due next month",
  },
  {
    label: "Assessment",
    detail: "Type 2 diabetes remains stable on current regimen.",
  },
  {
    label: "Plan",
    detail: "Continue metformin, repeat CMP, reinforce diet and exercise.",
  },
  {
    label: "Coding",
    detail: "E11.9, Z79.84, 99214 suggested",
  },
];
