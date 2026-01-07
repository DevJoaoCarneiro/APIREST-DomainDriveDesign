@api @user_management
Feature: Cadastro de Usuário
  Como um novo visitante
  Eu quero criar uma conta
  Para que eu possa acessar o sistema

  @Sucesso
  Scenario: Realizar cadastro de um novo usuário com sucesso
    Given que eu informo os seguintes dados de usuário:
      | Name           | Mail               | Password | Street    | Number | City      | State | ZipCode  |
      | Fulano Ciclano | fulano@email.com   | 123456   | Rua Teste | 100    | São Paulo | SP    | 01001000 |
    When eu envio uma requisição POST para "/api/users"
    Then o status code da resposta deve ser 200
    And o corpo da resposta "status" deve ser "Success"
    And o corpo da resposta "message" deve ser "User created successfully"