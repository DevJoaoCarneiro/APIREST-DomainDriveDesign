#language: pt-br
Funcionalidade: Cadastro de Usuario
    Como um novo visitante
    Eu quero me cadastrar no sistema
    Para acessar as funcionalidades restritas

@Sucesso
Cenario: 1. Cadastro com sucesso
    Dado que eu tenho os seguintes dados do usuario:
        | Name | Mail           | Password |
        | Joao | joao@gmail.com | joao123  |
    Quando eu envio os dados para cadastro
    Entao o status da resposta deve ser 200
    E a mensagem deve ser "User created successfully"
    E o campo "Status" deve ser "Success"

@Falha
Cenario: 2. Erro no cadastro do usuario
    Dado que meu payload de usuario esta nulo
    Quando eu envio os dados para cadastro
    Entao o status da resposta deve ser 400
    E a mensagem deve ser "Parameters is empty or null"
    E o campo "Status" deve ser "invalid_argument" 
     