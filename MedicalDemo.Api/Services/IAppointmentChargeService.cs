using MedicalDemo.Api.DTOs;

namespace MedicalDemo.Api.Services;

public interface IAppointmentChargeService
{
    AppointmentChargeDto GetByAppointmentId(int appointmentId);

    AppointmentChargeDto Create(int appointmentId, CreateAppointmentChargeDTO request);

    AppointmentChargeDto Update(int appointmentId, UpdateAppointmentChargeDTO request);

    AppointmentBillingSummaryDTO GetAppointmentBillingSummary(int appointmentId);

    IReadOnlyCollection<ChargeReviewItemDTO> GetReviewQueue();
}
