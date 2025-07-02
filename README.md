# PSI1623R_TiagoOliveira_2223234 | FinSync

![FinSync](https://raw.githubusercontent.com/tigokraft/PSI1623R_TiagoOliveira_2223234/refs/heads/main/docs/Screenshot%202025-07-02%20091505.png)

Aplicação de gestão financeira pessoal, composta por:

- **API back-end** desenvolvida em ASP.NET Core 8.0.
- **Cliente desktop** desenvolvido com Windows Forms em C#.

Permite ao utilizador gerir rendimentos, despesas e objectivos financeiros através de uma interface moderna e funcionalidades robustas.

## ✨ Funcionalidades

- Registo e autenticação de utilizadores (com JWT).
- Registo e consulta de rendimentos e despesas.
- Definição e acompanhamento de objectivos financeiros.
- Interface intuitiva com componentes Guna.UI2.

## 🗂 Estrutura

```
src/
├── api/        # ASP.NET Core Web API
└── Client/     # Aplicação Windows Forms
```

## 🧰 Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/)
- SQL Server Express ou LocalDB
- [EF](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

## ⚙️ Configuração

### API

1. Configurar `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=nome_da_base;Trusted_Connection=True;"
   }
   ```

2. Executar:
   ```bash
   dotnet ef database update
   ./starter.bat
   ```

A API estará acessível em `http://localhost:5034`.

### Cliente

1. Abrir `src/Client/login.sln` no Visual Studio.
2. Adicionar manualmente as referências a `Guna.UI2.WinForms.dll` e `Guna.UI.WinForms.dll`.
3. Compilar e executar.

## 📑 Endpoints Principais

| Endpoint            | Método | Descrição                   |
|---------------------|--------|-----------------------------|
| /api/users/login    | POST   | Autenticação                |
| /api/users/register | POST   | Registo                     |
| /api/incomes        | GET    | Listagem de rendimentos     |
| /api/expenses       | GET    | Listagem de despesas        |
| /api/goals          | GET    | Objectivos financeiros      |

## 🔐 Autenticação

Após o login, é devolvido um token JWT. Este deve ser incluído nos cabeçalhos de cada pedido autenticado:
```
Authorization: Bearer <token>
```

## 🧪 Testes

Pode utilizar [Postman](https://www.postman.com/) para testar os endpoints da API com o token JWT.

## 📜 Licença

Distribuído sob a licença MIT. Ver o ficheiro `LICENSE`.
