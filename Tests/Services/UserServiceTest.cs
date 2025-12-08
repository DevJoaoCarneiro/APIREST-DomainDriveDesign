using Application.Request;
using Application.Service;
using Domain.entities;
using Domain.Entities.Embeded;
using Domain.Interfaces;
using Domain.Repository;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Tests;

public class UserServiceTest
{

    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ISecurityService _userServices = Substitute.For<ISecurityService>();
    private readonly UserService _service;

    public UserServiceTest()
    {
        _service = new UserService(_userRepository, _userServices);
    }

    [Fact]
    public async void Should_Create_New_User_Corretly()
    {
        var request = new UserRequestDTO
        {
            Name = "Joao Victor",
            Mail = "joao@gmail.com",
            password = "joao123@#$"
        };

        var PasswordHashMock = "HIE2233";
        _userServices.HashPassword(request.password).Returns(PasswordHashMock);


        var result = await _service.createUser(request);

        Assert.Equal("User created successfully", result.Message);
        Assert.Equal("Success", result.Status);
        Assert.Equal(request.Name, result.Data.Name);
        Assert.Equal(request.Mail, result.Data.Mail);

    }

    [Fact]
    public async Task Should_Return_Invalid_Argument_When_Request_Is_Null()
    {
        UserRequestDTO? request = null;

        var result = await _service.createUser(request);

        Assert.Equal("Parameters is empty or null", result.Message);
        Assert.Equal("invalid_argument", result.Status);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task CreateUser_WhenRepositoryThrowsException_ReturnsErrorResponse()
    {

        var request = new UserRequestDTO
        {
            Name = "Joao Victor",
            Mail = "joao@gmail.com",
            password = "joao123@#$"
        };

        _userServices.HashPassword(Arg.Any<string>())
                       .Returns("hash123");

        _userRepository.AddAsync(Arg.Any<Domain.entities.User>())
                       .ThrowsAsync(new Exception("DB error"));

        var result = await _service.createUser(request);

        Assert.Equal("error", result.Status);
        Assert.Null(result.Data);

    }

    [Fact]
    public async Task Should_Return_All_Users_Corretly()
    {

        var address = new Address
        {
            Street = "Rua Teste",
            Number = "123",
            City = "Cidade X",
            State = "Estado Y",
            ZipCode = "00000-000"
        };
        var user = new User
        {
            Name = "Joao",
            Mail = "joao@gmail.com",
            PasswordHash = "hash123",
            UserAddress = address
        };

        var userList = new List<User>
        {
            new User
        {
            Name = "Joao",
            Mail = "joao@gmail.com",
            PasswordHash = "hash123",
            UserAddress = address
        },
            new User
        {
            Name = "Maria",
            Mail = "maria@gmail.com",
            PasswordHash = "hash123",
            UserAddress = address
        }
        };

        _userRepository.GetAllAsync()
                       .Returns(userList);


        var result = await _service.findAllUser();

        Assert.Equal("Users retrieved successfully", result.Message);
        Assert.Equal("Success", result.Status);
        Assert.Equal("Joao", result.Data[0].Name);
        Assert.Equal("Maria", result.Data[1].Name);
        Assert.Equal("joao@gmail.com", result.Data[0].Mail);
        Assert.Equal("maria@gmail.com", result.Data[1].Mail);
    }

    [Fact]
    public async void Should_Return_BadRequest_WhenError()
    {
        var address = new Address
        {
            Street = "Rua Teste",
            Number = "123",
            City = "Cidade X",
            State = "Estado Y",
            ZipCode = "00000-000"
        };
        var user = new User
        {
            Name = "Joao",
            Mail = "joao@gmail.com",
            PasswordHash = "hash123",
            UserAddress = address
        };

        _userRepository.GetAllAsync()
                       .ThrowsAsync(new Exception("DB error"));
        var result = await _service.findAllUser();

        Assert.Equal("error", result.Status);
        Assert.Equal("An error occurred while retrieving users", result.Message);

    }
    [Fact]
    public async void Should_Return_Empty_List_When_No_Users_Found()
    {
        var userList = new List<User>();

        _userRepository.GetAllAsync()
                       .Returns(new List<User>());
        var result = await _service.findAllUser();

        Assert.Equal("No users found", result.Message);
        Assert.Equal("not_found", result.Status);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task FindUserById_ShouldReturnInvalidCredentials_WhenIdIsEmpty()
    {
        var emptyId = Guid.Empty;

        var result = await _service.findUserById(emptyId);

        Assert.Equal("invalid_credentials", result.Status);
        Assert.Equal("UserId is invalid", result.Message);

        await _userRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>());
    }

    [Fact]
    public async Task FindUserById_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();

        _userRepository.GetByIdAsync(userId).Returns(Task.FromResult<User?>(null));

        var result = await _service.findUserById(userId);

        Assert.Equal("not_found", result.Status);
        Assert.Equal("No users found", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task FindUserById_ShouldReturnSuccess_WhenUserExists_WithAddress()
    {
        var userId = Guid.NewGuid();

        var address = new Address
        {
            Street = "Rua Teste",
            Number = "123",
            City = "Cidade X",
            State = "Estado Y",
            ZipCode = "00000-000"
        };
        var user = new User
        {
            Name = "Joao",
            Mail = "joao@email.com",
            PasswordHash = "hash123",
            UserAddress = address
        };

        _userRepository.GetByIdAsync(userId).Returns(user);

        var result = await _service.findUserById(userId);

        Assert.Equal("Success", result.Status);
        Assert.Equal("User found successfully", result.Message);
        Assert.NotNull(result.Data);

        Assert.Equal("Joao", result.Data.Name);
        Assert.Equal("joao@email.com", result.Data.Mail);

        Assert.NotNull(result.Data.UserAddress);
        Assert.Equal("Rua Teste", result.Data.UserAddress.Street);
        Assert.Equal("Cidade X", result.Data.UserAddress.City);
    }

    [Fact]
    public async Task FindUserById_ShouldReturnError_WhenRepositoryThrowsException()
    {
        var userId = Guid.NewGuid();
        var errorMessage = "Database connection failed";

        _userRepository.GetByIdAsync(userId).ThrowsAsync(new Exception(errorMessage));

        var result = await _service.findUserById(userId);

        Assert.Equal("error", result.Status);
        Assert.Contains(errorMessage, result.Message);
        Assert.Null(result.Data);

    }
}
