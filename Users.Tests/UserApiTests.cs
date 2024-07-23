using System.Configuration;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Models;

namespace Users.Tests;

public class UserApiTests
{
    private HttpClient _httpClient;
    private const string _endpointURL = "http://localhost:5201";

    [OneTimeSetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
    }
    
    [Test, Order(1)]
    public async Task GetHelloWorld()
    {
        // Arrange
        var requestPath = "/api/Users/hello";
       
        // Act
        var response = await _httpClient.GetAsync(_endpointURL + requestPath);
        var stringResponse = await response.Content.ReadAsStringAsync();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stringResponse.Should().Be("Hello World!");
    }
    
    [Test, Order(2)]
    public async Task GetUsers()
    {
        // Arrange
        var requestPath = "/api/Users";
        
        // Act
        var response = await _httpClient.GetAsync(_endpointURL + requestPath);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<User[]>();
        
        // Assert
        Assert.NotNull(users);
        
        if (users != null)
            Assert.GreaterOrEqual(users.Length, 1);
    }
    
    [Test, Order(3)]
    public async Task InsertUser()
    {
        var random = new Random();
        var userId = random.Next(Int32.MaxValue);
        
        // Arrange
        var requestPath = "/api/Users";
        User userToCreate = new User { Id = userId, Email = "test@test.com.br", Name = "User999 User999" }; 
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(_endpointURL + requestPath, userToCreate);
        var usersResult = await response.Content.ReadFromJsonAsync<User>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        userToCreate.Should().BeEquivalentTo(usersResult);
    }

    [Test, Order(4)]
    public async Task UpdateUser()
    {
        // Arrange
        var getRequestPath = "/api/Users";
        
        // Act
        var getResponse = await _httpClient.GetAsync(_endpointURL + getRequestPath);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getUsers = await getResponse.Content.ReadFromJsonAsync<User[]>();
        var getUser = getUsers.FirstOrDefault();
        
        var userToUpdate = new User { Id = getUser.Id, Email = getUser.Email, Name = getUser.Name + " UPDATE"};
        var putRequestPath = $"/api/Users/{getUser.Id}";
        var putResponse = await _httpClient.PutAsJsonAsync(_endpointURL + putRequestPath, userToUpdate);
        
        // Assert
        putResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Test, Order(5)]
    public async Task DeleteUser()
    {
        // Arrange
        var getRequestPath = "/api/Users";
        
        // Act
        var getResponse = await _httpClient.GetAsync(_endpointURL + getRequestPath);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getUsers = await getResponse.Content.ReadFromJsonAsync<User[]>();
        var getUser = getUsers.FirstOrDefault();
        
        var deleteRequestPath = $"/api/Users/{getUser.Id}";
        var deleteResponse = await _httpClient.DeleteAsync(_endpointURL + deleteRequestPath);
        
        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
    }
}