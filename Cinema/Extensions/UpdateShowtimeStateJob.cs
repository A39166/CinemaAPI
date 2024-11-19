using CinemaAPI.Databases.CinemaDB;
using Quartz;

public class UpdateShowtimeStateJob : IJob
{
    private readonly DBContext _context;
    private readonly ILogger<UpdateShowtimeStateJob> _logger;

    public UpdateShowtimeStateJob(DBContext context, ILogger<UpdateShowtimeStateJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var currentDateTime = DateTime.Now;
            var currentTime = currentDateTime.TimeOfDay;
            // Cập nhật các suất chiếu sắp chiếu thành "Đang chiếu" (Status = 1)
            var upcomingShowtimes = _context.Showtimes
             .Where(st => st.State == 0)  // Suất chiếu có trạng thái là chưa bắt đầu
             .AsEnumerable()  // Chuyển sang client-side để sử dụng TimeOnly và các phương thức không hỗ trợ trong LINQ to Entities
             .Where(st => st.StartTime.ToTimeSpan() <= currentTime)  // So sánh StartTime với currentTime
             .ToList();

            foreach (var showtime in upcomingShowtimes)
            {
                showtime.State = 1; // Đặt trạng thái là "Đang chiếu"
            }

            // Cập nhật các suất chiếu đang chiếu thành "Đã chiếu" (Status = 2)
            var ongoingShowtimes = _context.Showtimes
                .Where(st => st.State == 1)  // Suất chiếu đang chiếu
                .AsEnumerable()  // Chuyển sang client-side
                .Where(st => st.EndTime.ToTimeSpan() <= currentTime)  // So sánh EndTime với currentTime
                .ToList();

            foreach (var showtime in ongoingShowtimes)
            {
                showtime.State = 2; // Đặt trạng thái là "Đã chiếu"
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi cập nhật trạng thái suất chiếu: {ex.Message}");
        }
    }
}
