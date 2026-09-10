# Mais que scaffolding. Sua API pronta para rodar em segundos. Gerador de Código Automático .NET 10.

![.NET Version](https://img.shields.io/badge/.NET-10.0-purple)
![Status](https://img.shields.io/badge/Status-BETA-orange)
![License](https://img.shields.io/badge/License-MIT-blue)

Um gerador de código automatizado desenvolvido para acelerar a criação de APIs em .NET 10. 

O projeto elimina a configuração manual repetitiva ao gerar instantaneamente a estrutura base do projeto, separando as responsabilidades de acordo com as melhores práticas de Clean Architecture e CQRS (ou quase isso).

## Funcionalidades

- **Scaffolding Estrutural:** Criação automática de toda a estrutura de pastas e arquivos da API.
- **Clean Architecture & CQRS:** O código gerado é estritamente dividido nas camadas API, Domain e Application, estruturado em Commands e Queries.
- **Casos de Uso Prontos:** Geração das operações CRUD completas (GET, GET ALL, POST, INSERT, DELETE) utilizando LINQ para consultas.
- **Modelagem Dinâmica:** Permite definir o nome do contexto de banco de dados, além das classes e seus respectivos atributos.
- **Resolução de Dependências:** Mapeamento e inclusão automática dos usings corretos em cada camada para garantir a compilação imediata.
- **Interface Flexível:** Pode ser executado via Prompt de Comando (CLI) ou integrado via API.

## Como Utilizar

1. **Nome da Solução:** Defina o nome base do projeto/solution.
2. **Definição de Contexto:** Informe o nome do contexto que será utilizado.
3. **Modelagem de Entidades:** Liste as classes e seus atributos conforme a necessidade do negócio.
4. **Execução:** O algoritmo processará as entradas e gerará a estrutura do projeto.
5. **Integração:** Copie o código gerado para o seu repositório destino.

### Rodando o Projeto Gerado

Como o projeto exportado já vem com a estrutura de banco de dados configurada, para rodar a aplicação basta:

1. Inserir a sua Connection String no arquivo `appsettings.json` (ou preferivelmente no `user secrets`) do projeto gerado.
2. Abrir Package Manager Console.
3. Apontar a camada para `Infraestructure`, ainda no Package Manager Console.  
4. Criar a migration executando o comando: `add-migration Initial`.
5. Atualizar o banco de dados executando: `update-database`.

## Arquitetura e Considerações

O código gerado entrega uma base funcional altamente coesa e de baixo acoplamento. Desenvolvedores devem observar que:

- **Base Arquitetural:** O uso de CQRS e Clean Architecture exige disciplina na manutenção das camadas. A ferramenta fornece a estrutura, mas adaptações podem ser necessárias dependendo da complexidade do domínio.
- **Personalização:** Os casos de uso cobrem operações básicas. Cenários específicos de negócio ou consultas de alta performance devem ser adaptados manualmente após a geração.

## Como Contribuir

1. Faça o Fork do projeto
2. Crie uma Branch para sua modificação (git checkout -b feature/NovaFuncionalidade)
3. Faça o Commit das alterações (git commit -m 'Add: nova funcionalidade X')
4. Faça o Push para a Branch (git push origin feature/NovaFuncionalidade)
5. Abra um Pull Request
