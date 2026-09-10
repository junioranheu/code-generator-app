# Mais que scaffolding. Sua API pronta para rodar em segundos. Gerador de Código Automático .NET 10

![.NET Version](https://img.shields.io/badge/.NET-10.0-purple)
![Status](https://img.shields.io/badge/Status-BETA-orange)
![License](https://img.shields.io/badge/License-MIT-blue)

Ferramenta automatizada para acelerar a criação de APIs em .NET 10. O projeto elimina a configuração manual repetitiva gerando instantaneamente a estrutura base, separando as responsabilidades de acordo com as práticas de Clean Architecture e CQRS.

## Funcionalidades

- **Scaffolding Estrutural:** Criação automática de toda a estrutura de pastas e arquivos da API.
- **Clean Architecture & CQRS:** O código gerado é estritamente dividido nas camadas API, Domain e Application, estruturado em Commands e Queries.
- **Casos de Uso Prontos:** Geração das operações CRUD completas (GET, GET ALL, POST, INSERT, DELETE) utilizando LINQ para consultas.
- **Modelagem Dinâmica:** Permite definir o nome do contexto de banco de dados, além das classes e seus respectivos atributos.
- **Resolução de Dependências:** Mapeamento e inclusão automática dos usings corretos em cada camada para garantir a compilação imediata.
- **Interface Flexível:** Pode ser executado via Prompt de Comando (CLI) ou integrado via API.

## Status de Validação do Código Gerado

A ferramenta é capaz de gerar um projeto íntegro e compilável. Abaixo está o status atual de testes das saídas geradas:

- [x] Criação automática da estrutura de todas as camadas (API, Domain, Application, Infrastructure).
- [x] Criação automática de Entidades com base na modelagem fornecida.
- [x] Criação automática de Controllers roteados e injetados corretamente.
- [x] Geração automática das classes e interfaces dos Use Cases.
- [x] Instalação automática dos pacotes NuGet necessários no projeto gerado.
- [x] Conexão e referenciamento entre os projetos da Solution (Project Dependencies).
- [x] Compilação do projeto (Build realizado com sucesso).
- [x] Geração de Migrations via Entity Framework.
- [ ] Execução/teste manual dos Use Cases gerados.
- [ ] Criação automática de testes unitários e de integração.

## Como Utilizar

1. **Nome da Solução:** Defina o nome base do projeto/solution.
2. **Definição de Contexto:** Informe o nome do contexto que será utilizado.
3. **Modelagem de Entidades:** Liste as classes e seus atributos conforme a necessidade do negócio.
4. **Execução:** O algoritmo processará as entradas e gerará a estrutura do projeto.
5. **Integração:** Copie o código gerado para o seu diretório de destino.

### Rodando o Projeto Gerado

Como o projeto exportado já vem com a estrutura de banco de dados configurada, para rodar a aplicação basta:

1. Inserir a sua Connection String no arquivo `appsettings.json` (ou preferencialmente no `User Secrets`) do projeto gerado.
2. Abrir o Package Manager Console (PMC).
3. Alterar o "Default project" no PMC para a camada de `Infrastructure`.
4. Criar a migration executando o comando: `add-migration Initial`.
5. Atualizar o banco de dados executando: `update-database`.

## Como Contribuir

1. Faça o Fork do projeto
2. Crie uma Branch para sua modificação (`git checkout -b feature/NovaFuncionalidade`)
3. Faça o Commit das alterações (`git commit -m 'Add: nova funcionalidade X'`)
4. Faça o Push para a Branch (`git push origin feature/NovaFuncionalidade`)
5. Abra um Pull Request
