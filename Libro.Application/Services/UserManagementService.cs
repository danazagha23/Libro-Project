using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserManagementService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

            return usersDTO;
        }
        public async Task<UserDTO> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            var userDTO = _mapper.Map<UserDTO>(user);

            return userDTO;
        }
        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            var userDTO = _mapper.Map<UserDTO>(user);

            return userDTO;
        }
        public async Task<IEnumerable<UserDTO>> GetUsersByRoleAsync(string role)
        {
            var users = await _userRepository.GetUsersByRoleAsync(role);
            var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

            return usersDTO;
        }
        public async Task<UserDTO> CreateUserAsync(UserDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);
            await _userRepository.CreateUserAsync(user);

            return _mapper.Map<UserDTO>(user);
        }
        public async Task<UserDTO> UpdateUserAsync(int userId, UserDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);
            await _userRepository.UpdateUserAsync(userId, user);

            return _mapper.Map<UserDTO>(user);
        }
        public async Task<bool> DeleteUserAsync(int userId)
        {
            return await _userRepository.DeleteUserAsync(userId);
        }
        public async Task<bool> AssignUserRole(int userId, UserRole userRole)
        {
            User user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.Role = userRole;
            await _userRepository.UpdateUserAsync(userId, user);

            return true;
        }
    }
}
