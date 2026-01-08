@api @user_management
Feature: User Registration
  As a new visitor
  I want to create an account
  So that I can access the system

  @Success
  Scenario: Successfully register a new user
    Given I provide the following user data:
      | Name           | Mail             | Password | Street    | Number | City      | State | ZipCode  |
      | Fulano Ciclano | fulano@email.com | 123456   | Rua Teste | 100    | São Paulo | SP    | 01001000 |
    When I send a POST request to "/api/users"
    Then the response status code should be 200
    And the response body "status" should be "Success"
    And the response body "message" should be "User created successfully"