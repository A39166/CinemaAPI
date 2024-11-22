using System.ComponentModel;

namespace CinemaAPI.Enums
{
    public enum ErrorCode
    {
        [Description("Failed")]
        FAILED = -1,
        [Description("Success")]
        SUCCESS = 0,
        [Description("Token Invalid")]
        TOKEN_INVALID = 2,
        [Description("System error")]
        SYSTEM_ERROR = 3,
        [Description("Database failed")]
        DB_FAILED = 4,
        [Description("Thư mục chứa ảnh chưa được cấu hình")]
        FOLDER_IMAGE_NOT_FOUND = 5,
        [Description("Định dạng tập tin không được hỗ trợ")]
        DOES_NOT_SUPPORT_FILE_FORMAT = 6,
        [Description("Not found")]
        NOT_FOUND = 7,
        [Description("Invalid parameters")]
        INVALID_PARAM = 8,
        [Description("Exists")]
        EXISTS = 9,
        [Description("Key cert invalid")]
        INVALID_CERT = 10,
        [Description("Bad request")]
        BAD_REQUEST = 400,
        [Description("Unauthorization")]
        UNAUTHOR = 401,

        [Description("User locked")]
        USER_LOCKED = 20,
        [Description("Otp invalid")]
        OTP_INVALID = 21,
        [Description("Otp expired")]
        OTP_EXPIRED = 22,
        [Description("Tài khoản đã bị khóa. Vui lòng liên hệ Admin để biết thêm chi tiết")]
        ACCOUNT_LOCKED = 23,

        [Description("Đã có thể loại này")]
        DUPLICATE_GENRE = 24,

        [Description("Tài khoản không tồn tại. Vui lòng kiểm tra lại")]
        ACCOUNT_NOTFOUND = 25,
        [Description("Email hoặc Mật khẩu sai ! Vui lòng thử lại")]
        WRONG_LOGIN = 26,
        [Description("Tài khoản đã có")]
        DUPLICATE_EMAIL = 27,
        [Description("Mật khẩu không khớp")]
        MATCH_PASS = 28,
        [Description("Đã có quốc gia này")]
        DUPLICATE_REGION = 29,

        [Description("Không tìm thấy phim!")]
        MOVIE_NOTFOUND = 30,

        [Description("Phim đã tồn tại!")]
        DUPLICATE_MOVIE = 31,

        [Description("Không tìm thấy người dùng!")]
        USER_NOTFOUND = 32,

        [Description("Đã có rạp chiếu này!")]
        DUPLICATE_SCREEN = 33,

        [Description("Không tìm thấy phòng chiếu!")]
        SCREEN_NOT_FOUND = 34,

        [Description("Tài khoản này đã bị khóa!")]
        LOCKED_ACCOUNT = 35,

        [Description("Suất chiếu bị trùng lịch!")]
        DUPLICATED_SHOWTIME = 36,

        [Description("Không có dữ liệu cần update!")]
        NO_DATA = 37,

        [Description("Chỉ được cập nhật suất chiếu khi trạng thái là Sắp chiếu!")]
        CANT_UPDATE_SHOWTIME = 38,

        [Description("Phòng chiếu đang có suất chiếu đang chiếu!")]
        CANT_UPDATE_SCREEN = 39,

        [Description("Đã có giá vé này!")]
        DUPLICATE_TICKET_PRICE = 40,

        [Description("Tổng thời gian chiếu ít hơn thời lượng phim!")]
        INVALID_TIME = 41,

    }
}
