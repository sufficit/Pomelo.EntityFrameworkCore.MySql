# Pomelo de compatibilidade com conector corrigido

Objetivo: usar o fork Sufficit para manter EF Core 3.1/netstandard2.0 com MySqlConnector >=2.6.1, sem supressões do advisory.

## Checkpoints

1. [concluído] Criar perfil de build fixado em origin/3.2-maint (520f4dbc7733ca5f97122fdf0dddfcf77e1b6cef), portar namespaces/APIs e empacotar a partir de fonte.
2. [concluído] Validar consultas, modelo, CRUD e transações do provider em MariaDB descartável e execução em Mono/.NET Framework.
3. [em andamento] Recompilar o perfil congelado EFData, alinhar dependências dos consumidores e validar os três frameworks do Standard e audit transitivo.
4. [pendente] Registrar atividades/proveniência, revisar arquivos próprios, commit/push na main nos repositórios afetados e conferir CI/publicação.

## Restrições

- Manter target netstandard2.0; não substituir o provider 3.2 por EF Core 10.
- Gerar DLL real do fork; não editar nuspec de binário antigo para prometer compatibilidade.
- Chave de assinatura Pomelo e versão de assembly 3.2.7.0 preservadas. Contratos DbConnection/DbCommand do EFData serão conferidos; a implementação interna troca o namespace MySql.Data.MySqlClient por MySqlConnector.
- Snapshot legado gerado em obj com commit fixado; main EF Core 10 continua no caminho existente. Atualizar também seu piso de MySqlConnector na origem.
- Sem produção: banco novo/descartável local, nenhum deploy. Preservar alterações paralelas.
- Publicação e commit/push são evidências separadas; consumir pacote local com proveniência se o feed bloquear.

Perfil legado compilado/empacotado como 3.2.8-sufficit.1. Identidade/chave, recursos, modelo e conector verificados offline tanto em .NET 10 quanto em Mono/net472. Alinhados DI/logging 8 e dependências BCL/configuração que o novo conector exige. Piso do conector também atualizado na linha principal EF Core 10.

## Regressões do provider

Provider original 3.2.7 + conector 2.6.1 falhou com TypeLoadException; mesmo probe com provider recompilado passou. MariaDB próprio em /tmp e porta loopback aleatória: CRUD/Unicode/Guid binary16/decimal/datetime/bool/null/commit/rollback aprovados em .NET 10 (I/O assíncrono) e Mono net472 (I/O síncrono). Modelo/identidade/recursos aprovados em ambos.

Limitação encontrada: o EF Core 3.1 no Mono instalado falha no helper assíncrono DbConnectionExtensions por identidade duplicada de ValueTask. Não aplicar workaround no provider nem reportar async Mono como aprovado; validar o carregamento também em runner Windows/.NET Framework.

A validação da linha principal encontrou XML/Pkcs de build antigos vulneráveis (warnings-as-errors). Atualizados o servicing XML 9 e SourceLink 10 no produtor para executar os testes sem desativar audit.

O perfil EFData com namespaces atualizados compilou netstandard2.0 em clone isolado (Base como irmão). A linha moderna passou 74 testes unitários depois de atualizar MySqlConnector e os pacotes de build vulneráveis. O push do fork nesta etapa habilita a validação nativa Windows/Framework antes da integração final; os demais checkpoints seguem pendentes.
