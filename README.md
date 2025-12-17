# 🔐 Secure Authentication API

![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen)
![.NET](https://img.shields.io/badge/.NET%208-ASP.NET%20Core-purple)
![Docker](https://img.shields.io/badge/Docker-Compose-blue)
![Kafka](https://img.shields.io/badge/Kafka-Event%20Driven-black)
![SonarCloud](https://img.shields.io/badge/Quality-SonarCloud-orange)

Sistema de autenticação desenvolvido em .NET, seguindo os princípios de **Domain-Driven Design (DDD)** e **SOLID**, com foco em segurança, escalabilidade e boas práticas de engenharia de software.

Este projeto foi pensado para simular um cenário real de mercado, incluindo autenticação tradicional, login social, refresh token, recuperação de senha assíncrona com mensageria e validações de qualidade de código.

---

## 🚀 Principais Tecnologias e Conceitos

* **.NET (ASP.NET Core)**
* **Domain-Driven Design (DDD)**
* **Princípios SOLID**
* **Kafka** (Processamento assíncrono)
* **Docker & Docker Compose** (Infraestrutura local)
* **SonarCloud** (Análise estática e qualidade de código)
* **Mailtrap** (Disparo e validação de e-mails em ambiente de desenvolvimento)
* **JWT + Refresh Token**
* **Swagger / OpenAPI** (Documentação da API)

---

## 🧱 Arquitetura

Este projeto foi estruturado seguindo **Domain-Driven Design (DDD)** e **Clean Architecture**, garantindo baixo acoplamento, alta coesão e facilidade de manutenção.

### 📂 Estrutura da Solução

```text
Solution
│
├── Api
│   ├── Controllers
│   ├── Program.cs
│   ├── appsettings.json
│   └── Api.http
│
├── Application
│   ├── Interfaces
│   ├── Services
│   ├── Request (DTOs de entrada)
│   ├── Response (DTOs de saída)
│   ├── Validators
│   └── Helpers
│
├── Domain
│   ├── Entities
│   ├── Models
│   └── Interfaces
│
├── Infrastructure
│   ├── Context
│   ├── Persistence
│   ├── Repositories
│   ├── Messaging (Kafka Producers/Consumers)
│   ├── ExternalServices (Email, Google Auth, etc)
│   ├── Migrations
│   └── Services
│
└── Tests
    ├── Controller
    ├── Services
    └── Infrastructure
```
## 📨 Processamento Assíncrono com Kafka

Para simular um cenário real de alta escala, o projeto utiliza **Apache Kafka** para processamento assíncrono de eventos.

### 🔄 Caso de Uso: Redefinição de Senha
1.  O usuário solicita a redefinição de senha.
2.  A API publica um evento no Kafka.
3.  Um *Consumer* processa a mensagem.
4.  O serviço de e-mail é acionado de forma assíncrona.

**✅ Benefícios:**
* Melhora de performance da API.
* Desacoplamento entre serviços.
* Maior escalabilidade.
* Padrão amplamente utilizado em arquiteturas distribuídas.

### 📧 Envio de E-mails
O envio de e-mails foi implementado de forma desacoplada e orientada a interfaces.
* **Mailtrap** é utilizado em ambiente de desenvolvimento.
* Permite validar fluxos sem disparar e-mails reais.
* Integrado ao Kafka no fluxo de redefinição de senha.

---

## 📌 Endpoints Disponíveis

A API está documentada utilizando Swagger (OpenAPI) e pode ser acessada após subir o projeto.

### 🔑 Auth

| Método | Endpoint | Descrição |
| :--- | :--- | :--- |
| `POST` | `/api/auth` | Login com e-mail e senha |
| `POST` | `/api/auth/refresh-token` | Geração de novo token de acesso |
| `POST` | `/api/auth/google` | Login com Google |
| `POST` | `/api/auth/forgot-password` | Solicitação de redefinição de senha |
| `POST` | `/api/auth/reset-password` | Conclusão da redefinição de senha |

### 👤 User

| Método | Endpoint | Descrição |
| :--- | :--- | :--- |
| `POST` | `/api/users` | Criação de usuário |
| `GET` | `/api/users` | Listagem de usuários |
| `GET` | `/api/users/{userId}` | Buscar usuário por ID |

---

## 🐳 Docker & Infraestrutura

A infraestrutura do projeto foi preparada para execução local utilizando Docker.

**📦 Serviços Conteinerizados:**
* Kafka
* Zookeeper
* Kafka UI (visualização e monitoramento de tópicos)

Isso garante um ambiente previsível, reproduzível e alinhado com práticas modernas de desenvolvimento.

---

## ▶️ Como Rodar o Projeto

### 📋 Configuração do `appsettings.json`

Antes de executar o projeto, é necessário configurar corretamente o arquivo `appsettings.json`. Abaixo está um exemplo com todos os campos que precisam ser preenchidos.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Authentication": {
    "Google": {
      "ClientId": "SEU_GOOGLE_CLIENT_ID"
    }
  },
  "Jwt": {
    "Secret": "SUA_CHAVE_SECRETA_JWT_COM_PELO_MENOS_32_CARACTERES"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.mailtrap.io",
    "Port": 587,
    "SenderName": "Nome do Remetente",
    "SenderEmail": "no-reply@seudominio.com",
    "Username": "USUARIO_DO_MAILTRAP",
    "Password": "SENHA_DO_MAILTRAP"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "Topics": {
      "ResetRequestEventDTO": "nome do seu topico",
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=Nome do Host;Port= Porta do banco ;Database=Nome database;Username=Usuario;Password=Sua senha"
  }
}
```
### 🔎 Explicação dos Campos

* `Authentication:Google:ClientId`: Client ID gerado no Google Cloud Console para login social.
* `Jwt:Secret`: Chave secreta usada para assinar os tokens JWT. Deve ser forte e mantida em segredo.
* `EmailSettings`: Configurações de SMTP (Mailtrap em ambiente de desenvolvimento).
* `Kafka:BootstrapServers`: Endereço do broker Kafka.
* `Kafka:Topics`: Nomes dos tópicos utilizados pela aplicação (`ResetRequestEventDTO`, `UserCreatedEvent`).
* `ConnectionStrings:DefaultConnection`: String de conexão com o banco de dados.

---

## 🗄️ Banco de Dados & Migrations

O projeto utiliza **Entity Framework Core** com Migrations para controle de versão do banco de dados.

### ✅ Benefícios das Migrations
* **Versionamento do schema.**
* **Evolução segura do banco.**
* **Facilidade de setup em novos ambientes.**

As migrations estão localizadas na camada de **Infrastructure**, mantendo o domínio isolado de detalhes de persistência.

---

## 🎯 Objetivo do Projeto

Este projeto foi desenvolvido com foco em:

1.  Demonstrar domínio em .NET e arquitetura de software.
2.  Aplicar DDD, SOLID e Clean Architecture.
3.  Utilizar mensageria com Kafka em fluxos críticos.
4.  Garantir qualidade de código com SonarCloud.
5.  Simular um ambiente real de produção.

---
## **Link do Sonar Qube:** [aqui o link](https://sonarcloud.io/summary/new_code?id=DevJoaoCarneiro_APIREST-DomainDriveDesign&branch=main)

## 👨‍💻 Autor

Desenvolvido por **João Carneiro**

*Back-end Developer*
`.NET` | `Arquitetura` | `Mensageria` | `Clean Code`

> ⭐ **Se este projeto foi útil ou interessante para você, considere deixar uma estrela no repositório!**
