# OrderManagement

Um sistema moderno de gerenciamento de pedidos construído com .NET 8 e arquitetura limpa (Clean Architecture). O projeto oferece uma API RESTful para gerenciar clientes, produtos e pedidos, com suporte a eventos de domínio, cache, mensageria e persistência híbrida.

## 🚀 Funcionalidades

- **Gerenciamento de Clientes**: CRUD completo de clientes com validações de domínio
- **Gerenciamento de Produtos**: Controle de inventário e produtos
- **Gerenciamento de Pedidos**: Criação, confirmação, processamento, envio e cancelamento de pedidos
- **Eventos de Domínio**: Publicação de eventos via RabbitMQ para integrações
- **Cache**: Redis para otimização de performance
- **Persistência Híbrida**: SQL Server para dados relacionais e MongoDB para dados não estruturados
- **Autenticação**: JWT Bearer Token
- **Monitoramento**: Health Checks e logs estruturados com Serilog
- **Documentação**: Swagger/OpenAPI

## 🛠️ Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **ASP.NET Core** - API Web
- **Entity Framework Core** - ORM para SQL Server
- **MongoDB.Driver** - Driver para MongoDB
- **MassTransit** - Framework de mensageria (RabbitMQ)
- **Redis** - Cache distribuído
- **Serilog** - Logging estruturado
- **Swashbuckle** - Documentação Swagger
- **xUnit** - Testes unitários
- **Docker** - Containerização

## 📋 Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) e Docker Compose
- (Opcional) [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

## 🚀 Instalação e Execução

### 1. Clonar o repositório

```bash
git clone git@github.com:andrefernandess/order_management.git
cd OrderManagement
```

### 2. Iniciar os serviços de infraestrutura

```bash
docker-compose up -d
```

Isso iniciará:
- SQL Server (porta 1433)
- MongoDB (porta 27017)
- Redis (porta 6379)
- RabbitMQ (porta 5672 e 15672 para management)

### 3. Configurar a aplicação

As configurações estão em `src/OrderManagement.API/appsettings.json`. As senhas padrão dos serviços Docker são:
- SQL Server: `SuaSenha@123`
- MongoDB: `SuaSenha@123` (usuário: admin)
- RabbitMQ: `SuaSenha@123` (usuário: admin)

### 4. Executar a aplicação

```bash
cd src/OrderManagement.API
dotnet run
```

A API estará disponível em `https://localhost:5001` (ou `http://localhost:5000`).

### 5. Acessar a documentação

- Swagger UI: `https://localhost:5001/swagger`
- Health Checks: `https://localhost:5001/health`

## 🧪 Testes

### Testes Unitários

```bash
dotnet test tests/OrderManagement.UnitTests/
```

### Testes de Integração

```bash
dotnet test tests/OrderManagement.IntegrationTests/
```

## 📁 Estrutura do Projeto

```
OrderManagement/
├── src/
│   ├── OrderManagement.API/           # Camada de apresentação (Controllers, Middlewares)
│   ├── OrderManagement.Application/   # Camada de aplicação (CQRS, DTOs, Behaviors)
│   ├── OrderManagement.Domain/        # Camada de domínio (Entities, Events, Value Objects)
│   └── OrderManagement.Infrastructure/ # Camada de infraestrutura (Persistence, Messaging, Caching)
├── tests/
│   ├── OrderManagement.UnitTests/     # Testes unitários
│   └── OrderManagement.IntegrationTests/ # Testes de integração
├── docker-compose.yml                 # Configuração dos serviços Docker
├── global.json                        # Versão do .NET SDK
└── OrderManagement.sln               # Solução .NET
```

## 🔧 Desenvolvimento

### Comandos Úteis

```bash
# Restaurar dependências
dotnet restore

# Build do projeto
dotnet build

# Executar migrations (se aplicável)
dotnet ef database update

# Limpar build
dotnet clean
```

### Arquivos de Configuração

- `appsettings.json` - Configurações da aplicação
- `appsettings.Development.json` - Configurações específicas para desenvolvimento
- `launchSettings.json` - Configurações do Visual Studio

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -am 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 📞 Suporte

Para dúvidas ou problemas, abra uma issue no repositório ou entre em contato com a equipe de desenvolvimento.</content>
<parameter name="filePath">c:\Projetos\OrderManagement\README.md
