using Application.Interfaces;
using Application.Request;
using Application.Response;
using Domain.entities;
using Domain.Entities.Embeded;
using Domain.Interfaces;
using Domain.Repository;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Service
{
    public class UserService : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly ISecurityService _securityService;

        public UserService(IUserRepository userRepository, ISecurityService securityService)
        {
            _userRepository = userRepository;
            _securityService = securityService;
        }

        public async Task<UserResponseDTO> createUser(UserRequestDTO userRequestDTO)
        {
            try
            {
                if (userRequestDTO == null)
                {
                    return new UserResponseDTO
                    {
                        Message = "Parameters is empty or null",
                        Status = "invalid_argument",
                        Data = null
                    };
                }

                string PasswordHash = _securityService.HashPassword(userRequestDTO.password);

                var newUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Name = userRequestDTO.Name,
                    Mail = userRequestDTO.Mail,
                    PasswordHash = PasswordHash,
                    UserAddress = new Address
                    {
                        Street = userRequestDTO.UserAddress.Street,
                        Number = userRequestDTO.UserAddress.Number,
                        City = userRequestDTO.UserAddress.City,
                        State = userRequestDTO.UserAddress.State,
                        ZipCode = userRequestDTO.UserAddress.ZipCode
                    }
                };

                await _userRepository.AddAsync(newUser);

                return new UserResponseDTO
                {
                    Message = "User created successfully",
                    Status = "Success",
                    Data = new UserData
                    {
                        Name = newUser.Name,
                        Mail = newUser.Mail
                    }
                };
            }
            catch (Exception ex)
            {

                return new UserResponseDTO
                {
                    Message = $"An error occurred: {ex.Message}",
                    Status = "error",
                    Data = null
                };
            }

        }

        public async Task<UserListResponseDTO> findAllUser()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();

                if (users == null || !users.Any())
                {
                    return new UserListResponseDTO
                    {

                        Message = "No users found",
                        Status = "not_found",
                        Data = new List<UserDataList>()
                    };
                }

                var userList = users.Select(user => new UserDataList
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Mail = user.Mail,
                    UserAddress = user.UserAddress != null ? new AddressResponseDTO 
                    {
                        Street = user.UserAddress.Street,
                        Number = user.UserAddress.Number,
                        City = user.UserAddress.City,
                        State = user.UserAddress.State,
                        ZipCode = user.UserAddress.ZipCode
                    }: null
                }).ToList();

                return new UserListResponseDTO
                {
                    Message = "Users retrieved successfully",
                    Status = "Success",
                    Data = userList
                };
            }
            catch (Exception)
            {
                return new UserListResponseDTO
                {

                    Message = "An error occurred while retrieving users",
                    Status = "error",
                    Data = null
                };

            }
        }

        public async Task<UserResponseDTO> findUserById(Guid userId)
        {

            try
            {
                if (userId == Guid.Empty)
                {
                    return new UserResponseDTO
                    {

                        Message = "UserId is invalid",
                        Status = "invalid_credentials",
                        Data = null
                    };
                }

                var response = await _userRepository.GetByIdAsync(userId);

                if (response == null)
                {
                    return new UserResponseDTO
                    {
                        Message = "No users found",
                        Status = "not_found",
                        Data = null
                    };
                }

                return new UserResponseDTO
                {
                    Message = "User found successfully",
                    Status = "Success",
                    Data = new UserData
                    {
                        UserId = response.UserId,
                        Name = response.Name,
                        Mail = response.Mail,
                        UserAddress = response.UserAddress != null ? new AddressResponseDTO
                        {
                            Street = response.UserAddress.Street,
                            Number = response.UserAddress.Number,
                            City = response.UserAddress.City,
                            State = response.UserAddress.State,
                            ZipCode = response.UserAddress.ZipCode
                        } : null
                    }
                };
            }
            catch (Exception ex)
            {
                return new UserResponseDTO
                {
                    Message = $"An error occurred: {ex.Message}",
                    Status = "error",
                    Data = null
                };
            }
            

        }
    }
}
