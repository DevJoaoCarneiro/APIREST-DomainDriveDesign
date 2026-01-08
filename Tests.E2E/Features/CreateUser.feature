# language: pt-br
@api @user_management
Funcionalidade: Cadastro de Usuário
  Como um novo visitante
  Eu quero criar uma conta
  Para que eu possa acessar o sistema

  @Sucesso
  Cenário: Realizar cadastro de um novo usuário com sucesso
    Dado que eu informo os seguintes dados de usuário:
      | Name           | Mail             | Password | Street    | Number | City      | State | ZipCode  |
      | Fulano Ciclano | fulano@email.com | 123456   | Rua Teste | 100    | São Paulo | SP    | 01001000 |
    Quando eu envio uma requisição POST para "/api/users"
    Então o status code da resposta deve ser 200
    E o corpo da resposta "status" deve ser "Success"
    E o corpo da resposta "message" deve ser "User created successfully"