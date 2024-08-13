# Gerador de Código Automático (API) .NET 8 **[BETA]**

Este projeto é um gerador de código automático desenvolvido em .NET 8. Ele facilita e acelera a criação de APIs, gerando a estrutura inicial do projeto, incluindo as camadas necessárias (API, Domain, Application) e os usings apropriados. Ideal para desenvolvedores que desejam evitar a configuração manual e repetitiva dessas estruturas.

## 🚀 Funcionalidades **[BETA]**

- **Geração de Estrutura de API**: Cria a estrutura inicial da API com todos os arquivos e pastas necessários, incluindo todos os casos de usos típicos (GET, GETALL, POST, INSERT, DELETE, etc).
- **Configuração de Contexto**: Permite definir o nome do contexto e suas respectivas classes e atributos.
- **Camadas Separadas**: Gera o código dividido corretamente nas camadas API, Domain e Application.
- **Mapeamento de Usings**: Inclui os usings corretos em cada camada para facilitar a integração.
- **Execução Flexível**: O algoritmo pode ser executado tanto via prompt de comando quanto por meio de uma API.

## 💡 Como Funciona

1. **Insira o Nome da Solução**: Defina o nome da solução do seu projeto.
2. **Defina o Contexto**: Informe o nome do contexto que será utilizado.
3. **Adicione Classes e Atributos**: Liste as classes e seus atributos conforme necessário.
4. **Geração do Código**: O algoritmo processa as informações e gera a estrutura inicial do projeto.
5. **Copie e Cole**: Copie o código gerado e cole em seu projeto existente.

## ⚠️ Aviso Importante sobre o Código Gerado

O código gerado por esta ferramenta segue um padrão estabelecido de **CQRS** com **Clean Architecture**. Este padrão foi escolhido para garantir uma estrutura organizada e de fácil manutenção. No entanto, é importante observar que:

- **Padrão CQRS com Clean Architecture**: O código gerado é baseado em CQRS e Clean Architecture, que ajudam a separar claramente as responsabilidades e melhorar a escalabilidade e manutenção do projeto. Esse padrão pode não ser adequado para todos os cenários e pode exigir ajustes conforme as necessidades específicas do seu projeto.

- **Personalização Necessária**: Embora a estrutura inicial seja fornecida, você pode precisar fazer modificações para alinhar o código gerado com os requisitos específicos do seu projeto. A ferramenta oferece uma base sólida, mas ajustes adicionais podem ser necessários para atender completamente aos seus objetivos e especificidades.

- **Cobertura de Use Cases**: Os casos de uso incluídos cobrem operações básicas, como GET, POST, INSERT, DELETE e GETALL, utilizando LINQ para consultas. Esses casos de uso são projetados para proporcionar uma base funcional, mas pode ser necessário expandir ou modificar esses casos para atender a casos de uso específicos do seu domínio.

Lembre-se de revisar e adaptar o código gerado conforme necessário para garantir que ele atenda às necessidades exclusivas do seu projeto e siga as melhores práticas para a sua situação particular.
