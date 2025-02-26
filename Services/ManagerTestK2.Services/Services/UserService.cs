﻿using AutoMapper;
using ManagerTestK2.Domain.Entities;
using ManagerTestK2.Domain.Interfaces;
using ManagerTestK2.Domain.Pagination;
using ManagerTestK2.Services.DTO;
using ManagerTestK2.Services.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace ManagerTestK2.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public UserService(IGenericRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PagedList<UserDTO>> GetAllUsers(int pageNumber, int pageSize)
        {
            var users = await _userRepository.GetAllPagination(pageNumber, pageSize);
            var usersDTO = _mapper.Map<List<UserDTO>>(users);

            return new PagedList<UserDTO>(usersDTO, pageNumber, pageSize, users.TotalCount);
        }

        public async Task<UserDTO> GetUserById(int idUser)
        {
            var user = await _userRepository.GetById(idUser);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> AddUser(UserDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);

            byte[] salt = new byte[128 / 8];
            user.Salt = GenerateSalt(salt);

            string cryptoPassword = EncryptPassword(userDTO.Password, user.Salt);
            user.Password = cryptoPassword;

            var newUser = await _userRepository.Create(user);
            return _mapper.Map<UserDTO>(newUser);
        }

        public async Task<UserDTO> UpdateUser(UserDTO userDTO)
        {
            var user = await _userRepository.GetById(userDTO.Id);

            if (user == null)
                return null;

            user.Name = userDTO.Name;
            user.Email = userDTO.Email;
            user.IsAdmin = userDTO.IsAdmin;

            byte[] salt = new byte[128 / 8];
            user.Salt = GenerateSalt(salt);

            string cryptoPassword = EncryptPassword(userDTO.Password, user.Salt);
            user.Password = cryptoPassword;

            var userUpdate = await _userRepository.Update(user);
            return _mapper.Map<UserDTO>(userUpdate);
        }

        public async Task<UserDTO> DeleteUser(int idUser)
        {
            var user = await _userRepository.GetById(idUser);

            if (user == null)
                return null;

            var userDeleted = await _userRepository.Remove(user);
            return _mapper.Map<UserDTO>(userDeleted);
        }

        private Byte[] GenerateSalt(byte[] salt)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private string EncryptPassword(string password, byte[] salt)
        {
            string cryptoPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return cryptoPassword;
        }
    }
}