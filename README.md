# Route Administration – Gerenciador de Rotas

Banco de dados utilizado: MongoDB.
Sistema baseado em: API WEB ASP .NET CORE, MVC com Razor Pages e C#.
Plataforma: Visual Studio 2022.

### Descrição

Website voltado para atender a demanda de uma planilha de dados onde consta dados de endereço completo, ordem de serviço, serviços a serem executados, entre outros.
O website possui um sistema de login que não permite nenhum acesso a outras páginas sem antes se autenticar com usuário e senha.
Para primeiro uso, o sistema gera uma senha e usuário temporário que, ao fazer o login pela primeira vez, redireciona o usuário a criar sua própria conta. Não é possível acessar as páginas do website com esse login e senha temporários.
Após criar sua conta, o usuário é enviado para a tela de login para que enfim possa logar e usufruir do website. Logo após o login, o usuário é redirecionado para página de upload da planilha de dados.
Após receber essa planilha de dados, é calculado uma rota de serviço. A rota é traçada de acordo com os CEPs que estão na planilha. Os CEPs são ordenados em ordem crescente quando a planilha é carregada no sistema.
O Sistema gera um documento de texto, onde estarão os dados solicitados pelo cliente.

### Etapas da usabilidade
Ao entrar no website, é exibido um menu superior onde está toda navegação necessária no site. A navegação segue uma rota direcional para que tudo funcione corretamente. A geração de rotas só será bem-sucedida seguindo os menus: Usuário (fazer o login), Enviar Arquivo, Pessoas (cadastro de pessoas), Cidades (cadastro de cidades), Equipes (cadastro de equipes), Rotas (gerador das rotas) e Baixar Arquivos (gerados nas rotas). Para que tudo funcione corretamente, é necessário executar o projeto com todas as APIs. Caso deseje executar todas ao mesmo tempo, basta ir nas propriedades da solução e mudar para Multiplos projetos, ficando: ApiCity, ApiEquip, ApiPerson e ApiUser sem depuração e Frontend com depuração. Caso deseje subir uma por vez, basta seguir a sequencia: ApiCity, ApiEquip, ApiPerson, ApiUser e Frontend, selecionando na parte superior da IDE do Visual Studio o nome da API e clicar na seta verde vazada, ou CTRL + F5.

### Os menus

O primeiro item do menu é “Usuário”. No primeiro acesso desta página é exibido uma tela de login. Não há a possibilidade de se criar uma conta. O sistema gera um usuário e senha temporário para que o usuário administrador possa entrar no sistema pela primeira vez. Após isso será necessário criar um usuário novo e assim começar a utilizar o sistema. Nessa mesma tela, ao estar logado com uma conta de administrador, será exibido um Gerenciamento de Usuário, onde a conta administrador poderá criar usuários, alterar senhas, nomes e o tipo, determinando que aquele usuário será um usuário comum ou administrador. Abaixo será exibido um Gerenciamento de Arquivos, onde uma conta administrativa poderá excluir ou fazer o download dos documentos de textos gerados anteriormente. Se a conta for do tipo de usuário comum, apenas será mostrada uma mensagem com o nome de usuário.

O segundo item do menu é “Enviar Arquivo”.  Nessa página será feito o envio da planilha de dados que será utilizada para gerar as rotas. Essa página é carregada automaticamente quando o usuário loga no sistema. A planilha de dados poderá conter a extensão .xls ou .xlsx. Não serão aceitos outros tipos de arquivos. O sistema gerencia toda parte de recepção dessa planilha, podendo ser carregada diversas vezes, porém apenas a última planilha enviada será utilizada para calcular as rotas.

O terceiro item do menu é “Pessoas”. Nessa página é exibida a lista de pessoas cadastradas no sistema. Em seu primeiro acesso nada será exibido. Nessa página  há um link onde poderá ser cadastrada às pessoas para que possam ser incluídas em uma equipe. Ainda, após a inserção da pessoa, é possível editar os dados, bem como removê-la do sistema. Nessa versão do sistema, não é possível cadastrar duas pessoas com o mesmo nome. Para atendimento de normas, as pessoas não são removidas do banco de dados, sendo feita apenas uma alteração de status.

O quarto item do menu é “Cidades”. Nessa página é exibida a lista de cidades cadastradas no sistema. Em seu primeiro acesso será exibido uma lista das cidades que constam na planilha de dados que foi carregada. Não haverá cidades duplicadas no sistema. Nessa página há um link onde poderá ser cadastrada as cidades que serão atendidas pelas equipes. Nessa primeira versão do sistema, somente as cidades pertencentes ao Estado de São Paulo serão admitidas no sistema. É possível ainda remover a cidade cadastrada.

O quinto item do menu é Equipes. Nessa página é exibida a lista de equipes cadastradas no sistema. Em seu primeiro acesso nada será exibido. Nessa página há um link onde poderá ser cadastrada às equipes que comporão o sistema. Na página de cadastro de equipe, não será possível cadastrar uma equipe sem possuir uma pessoa nela. Caso não apareça nenhuma pessoa para ser incluída nessa equipe, será necessário cadastrar mais pessoas no sistema. Ainda, é necessário escolher em qual cidade aquela equipe estará disponível para atuar. Após a inserção da equipe no sistema, é possível removê-la do sistema. Para atendimento de normas, as equipes não são removidas do banco de dados, sendo feita apenas uma alteração de status.

O sexto item do menu é “Rotas”. Nessa página se iniciará o processo de geração das rotas, bem como a geração do documento de texto com todas as informações necessárias. A primeira página exibida é a de seleção de colunas baseadas na planilha de dados previamente carregada. Das opções exibidas, as obrigatórias são: OS, cidade, base, serviço, endereço, número, complemento, CEP e bairro. Sem essas colunas selecionadas, o sistema não prosseguirá. Caso deseje incluir mais colunas que estão sendo exibidas, o sistema aceitará. Na próxima página será exibido os serviços que estão disponíveis para serem executados, conforme constam na planilha de dados. Não é possível prosseguir sem ao menos um serviço selecionado. Na próxima página será exibido as cidades que possuem aquele serviço escolhido na página anterior. Da mesma forma que a anterior, não é possível prosseguir sem a seleção de pelo menos uma cidade. A próxima página exibe uma lista de equipes cadastradas no sistema, desde que estejam classificadas para trabalhar na cidade escolhida na página anterior. É possível conferir os nomes das pessoas que estão naquela equipe. Caso nenhuma equipe esteja cadastrada para atuar naquela cidade, nenhuma equipe será exibida e não será possível progredir. Só é permitido o máximo de cinco serviços por equipe. Com todos esses dados, ao clicar no botão “Gerar Rota”, caso todos os dados estejam corretos, será mostrado uma página informando que o documento de texto foi gerado com sucesso e haverá um link para ir até a página de download do arquivo.

O último item do menu é “Baixar Arquivos”. Nessa página é exibido todos os documentos de texto que foram gerados pelo sistema. Em sua primeira exibição, caso nenhuma rota tenha sido gerada anteriormente, nada será mostrado. Ainda, é possível fazer o download dos arquivos gerados.

### Dependências

| __Opções__ | __Descrição__ | __Versão__ |
|-----|---------------------------|-----|
| 1. | EPPlus | 6.0.3 |
| 2. | MongoDB.Bson | 2.15.0 |
| 3. | MongoDB.Driver | 2.15.0 |
| 4. | Newtonsoft.Json | 13.0.1 |
| 5. |Spire.Doc | 10.4.5 |
| 6. | Swashbuckle.AspNetCore | 5.6.3 |
