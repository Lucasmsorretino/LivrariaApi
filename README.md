# SuperHero API

Uma API RESTful desenvolvida em ASP.NET Core 6.0 para gerenciar informações de super-heróis.

## 📋 Descrição

Este projeto é uma API web que permite criar, ler, atualizar e deletar (CRUD) informações de super-heróis. Foi desenvolvido como parte do curso de Análise e Desenvolvimento de Sistemas (ADS) na disciplina de Arquitetura de Desenvolvimento de APIs.

## 🚀 Tecnologias Utilizadas

- **ASP.NET Core 6.0** - Framework web
- **Entity Framework Core** - ORM para acesso a dados
- **SQL Server** - Banco de dados
- **Swagger/OpenAPI** - Documentação da API
- **CORS** - Política de compartilhamento de recursos

## 📁 Estrutura do Projeto

```
LivrariaApi/
├── Controllers/           # Controladores da API
│   └── SuperHeroController.cs
├── Data/                 # Contexto do banco de dados
│   └── DataContext.cs
├── Models/               # Modelos de dados
│   ├── SuperHero.cs
│   └── Employee.cs
├── Dependencies/         # Injeção de dependências
│   └── Dependencia_obj.cs
└── Program.cs           # Configuração da aplicação
```

## 🛠️ Funcionalidades

- ✅ CRUD completo de Super-Heróis
- ✅ Configuração CORS
- ✅ Documentação Swagger
- ✅ Injeção de Dependência
- ✅ Entity Framework com SQL Server

## 📦 Como Executar

### Pré-requisitos

- .NET 6.0 SDK
- SQL Server (Local ou Azure)
- Visual Studio 2022 ou VS Code

### Passos para execução

1. Clone o repositório:
```bash
git clone <url-do-repositorio>
cd LivrariaApi
```

2. Configure a string de conexão no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "sua-connection-string-aqui"
  }
}
```

3. Execute as migrações do banco de dados:
```bash
dotnet ef database update
```

4. Execute a aplicação:
```bash
dotnet run
```

5. Acesse a documentação Swagger em: `https://localhost:5001/swagger`

## 🔗 Endpoints da API

### SuperHero

- `GET /api/superhero` - Lista todos os super-heróis
- `GET /api/superhero/{id}` - Busca um super-herói por ID
- `POST /api/superhero` - Cria um novo super-herói
- `PUT /api/superhero/{id}` - Atualiza um super-herói
- `DELETE /api/superhero/{id}` - Remove um super-herói

## 📝 Modelo de Dados

### SuperHero
```csharp
{
  "id": 1,
  "nome": "Superman",
  "primeiroNome": "Clark",
  "ultimoNome": "Kent",
  "lugar": "Metropolis"
}
```

## 🤝 Contribuição

Este é um projeto acadêmico, mas contribuições são bem-vindas! Sinta-se à vontade para:

1. Fazer fork do projeto
2. Criar uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abrir um Pull Request

## 📄 Licença

Este projeto está sob licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👨‍💻 Autor

Desenvolvido como parte do curso de ADS - Arquitetura de Desenvolvimento de APIs.

---

⭐ Se este projeto te ajudou, não esqueça de dar uma estrela!
