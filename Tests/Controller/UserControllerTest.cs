using Application.Interfaces;
using NSubstitute;
using Api.controller;
using Application.Request;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Application.Response;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ExceptionExtensions;


namespace Tests.Controller
{
    public class UserControllerTest
    {
        private readonly IUserServices _userServices = Substitute.For<IUserServices>();
        private readonly UserController _controller;

        public UserControllerTest()
        {
            _controller = new UserController(_userServices);
        }

        [Fact]
        public async void Should_Create_User_Correctly_Via_Controller()
        {
            //Arrange
            var userRequest = new UserRequestDTO
            {
                Name = "Marcia Da Cruz",
                Mail = "marcia@gmail.com",
                password = "password123"
            };

            var userResponse = new UserResponseDTO
            {
                Message = "User created successfully",
                Status = "Success",
                Data = new UserData
                {
                    Name = "Marcia Da Cruz",
                    Mail = "marcia@gmail.com"

                }
            };

            _userServices.createUser(Arg.Any<UserRequestDTO>()).Returns(userResponse);

            //Act
            var response = await _controller.registerUser(userRequest);
            var okResult = Assert.IsType<OkObjectResult>(response);
            var resultValue = Assert.IsType<UserResponseDTO>(okResult.Value);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(userResponse.Message, resultValue.Message);
            Assert.Equal(userResponse.Status, resultValue.Status);
            Assert.Equal(userResponse.Data.Name, resultValue.Data.Name);
            Assert.Equal(userResponse.Data.Mail, resultValue.Data.Mail);

        }

        [Fact]
        public async void Should_Return_BadRequest_WhenError()
        {
            //Arrange
            var userRequest = new UserRequestDTO();

            _userServices.createUser(userRequest).ThrowsAsync(new Exception("Error"));

            //Act
            var result = await _controller.registerUser(userRequest);

            //Assert
            var StatusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, StatusCodeResult.StatusCode);

        }

        [Fact]
        public async void Should_Return_Invalid_Argument_When_Request_Is_null()
        {
            //Arrange
            var request = new UserRequestDTO();

            var serviceResponse = new UserResponseDTO
            {
                Message = "Parameters is empty or null",
                Status = "invalid_argument",
                Data = null
            };

            _userServices.createUser(Arg.Any<UserRequestDTO>()).Returns(serviceResponse);
            //Act
            var response = await _controller.registerUser(request);

            //Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(response);
            var responseValue = Assert.IsType<UserResponseDTO>(badRequest.Value);
            Assert.Equal(400, badRequest.StatusCode);
            Assert.Equal("invalid_argument", responseValue.Status);
            Assert.Equal("Parameters is empty or null", responseValue.Message);
        }

        [Fact]
        public async void Should_Get_All_Users_When_Is_Not_Empty()
        {
            //Arrange
            var userList = new List<UserDataList>{
               new UserDataList
                {
                    Name = "João",
                    Mail = "joao@gmail.com"
                },
                new UserDataList
                {
                    Name = "Maria",
                    Mail = "maria@gmail.com"
                }
            };

            var userResponse = new UserListResponseDTO
            {
                Message = "Users retrieved successfully",
                Status = "Success",
                Data = userList
            };

            _userServices.findAllUser().Returns(userResponse);

            //Act
            var response = await _controller.getAllUsers();
            var okResult = Assert.IsType<OkObjectResult>(response);
            var resultValue = Assert.IsType<UserListResponseDTO>(okResult.Value);

            //Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Success", resultValue.Status);
            Assert.Equal("Users retrieved successfully", resultValue.Message);
            Assert.Equal(2, resultValue.Data.Count);
            Assert.Equal("João", resultValue.Data[0].Name);
            Assert.Equal("joao@gmail.com", resultValue.Data[0].Mail);
        }

        [Fact]
        public async void Should_Return_Not_Found_When_List_Is_Empty()
        {
            //Arrange
            var userList = new List<UserDataList>();

            var userResponse = new UserListResponseDTO
            {
                Message = "No users found",
                Status = "not_found",
                Data = new List<UserDataList>()
            };

            _userServices.findAllUser().Returns(userResponse);

            //Act
            var response = await _controller.getAllUsers();

            var notFound = Assert.IsType<NotFoundObjectResult>(response);
            var resultValue = Assert.IsType<UserListResponseDTO>(notFound.Value);

            //Assert
            Assert.Equal(404, notFound.StatusCode);
            Assert.Equal("not_found", resultValue.Status);
            Assert.Equal("No users found", resultValue.Message);
            Assert.Empty(resultValue.Data);

        }

        [Fact]
        public async void Should_Return_500_When_Exception_Is_Thrown()
        {
            // Arrange
            _userServices.findAllUser()
                         .ThrowsAsync(new Exception("Db Error"));

            // Act
            var response = await _controller.getAllUsers();

            // Assert
            var result = Assert.IsType<ObjectResult>(response);

            Assert.Equal(500, result.StatusCode);
        }
    }
}
