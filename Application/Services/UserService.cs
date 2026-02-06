using Application.DTOs.RequestDTOs.User;
using Application.DTOs.ResponseDTOs.User;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    /// <summary>
    /// Service responsible for managing user accounts and data retrieval.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of all registered users in the system.
        /// </summary>
        /// <returns>A collection of user profile DTOs.</returns>
        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var listUsers = await _unitOfWork.UserRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(listUsers);
        }

        /// <summary>
        /// Retrieves a single user by their ID.
        /// </summary>
        public async Task<UserResponseDTO> GetUserByIdAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");

            return _mapper.Map<UserResponseDTO>(user);
        }

        /// <summary>
        /// Creates a new user (Admin function). Hashes password and supports AvatarUrl.
        /// </summary>
        public async Task<UserResponseDTO> CreateUserAsync(CreateUserDTO request)
        {
            // 1. Kiểm tra Email đã tồn tại chưa
            var isExist = await _unitOfWork.UserRepository.CheckEmailExist(request.Email);
            if (isExist) throw new Exception("Email is already registered.");

            // 2. Map dữ liệu từ DTO sang Entity
            var user = _mapper.Map<User>(request);

            // 3. Khởi tạo các giá trị mặc định
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;

            // 4. HASH PASSWORD (Bắt buộc)
            // Sử dụng BCrypt để mã hóa mật khẩu trước khi lưu
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 5. Lưu vào DB
            await _unitOfWork.UserRepository.CreateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserResponseDTO>(user);
        }

        /// <summary>
        /// Updates user profile (FullName, AvatarUrl). Does NOT allow Email update.
        /// </summary>
        public async Task<UserResponseDTO> UpdateUserAsync(Guid id, UpdateUserDTO request)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");

            // Chỉ cập nhật FullName và AvatarUrl nếu có dữ liệu gửi lên
            // AutoMapper đã được cấu hình (trong MappingProfile) để bỏ qua null,
            // hoặc bạn có thể gán thủ công để kiểm soát chặt chẽ hơn:

            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrEmpty(request.AvatarUrl))
                user.AvatarUrl = request.AvatarUrl;

            // Lưu ý: Tuyệt đối không update user.Email ở đây theo yêu cầu

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserResponseDTO>(user);
        }

        /// <summary>
        /// Changes the user's password. Requires the old password for verification.
        /// </summary>
        public async Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDTO request)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");

            // 1. Kiểm tra mật khẩu cũ (Verify Hash)
            bool isOldPassCorrect = BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash);
            if (!isOldPassCorrect)
            {
                throw new Exception("Incorrect old password.");
            }

            // 2. Hash mật khẩu mới và cập nhật
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Deletes a user permanently.
        /// </summary>
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");

            _unitOfWork.UserRepository.Delete(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}