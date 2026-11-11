# Pomelo netstandard2.0 com conector corrigido

Criado perfil de provider EF Core 3.1 a partir do commit fixado 520f4dbc7733ca5f97122fdf0dddfcf77e1b6cef (3.2-maint). Fontes/recursos são gerados em obj e compilados com MySqlConnector >=2.6.1, DI/logging 8, BCL/configuração declarados; chave original e AssemblyVersion 3.2.7.0 preservados. Pacote 3.2.8-sufficit.1. Nenhuma edição de IL/nuspec de DLL antiga.

Regressão: provider 3.2.7 original com 2.6.1 falhou com TypeLoadException; provider recompilado passou no mesmo probe. CRUD/Unicode/Guid binary16/decimal/datetime/bool/null/commit/rollback passaram em MariaDB novo, datadir temporário e porta loopback aleatória, tanto no .NET 10 (I/O async) como Mono/net472 (I/O sync). Loader/modelo/recursos/assinatura passaram nos dois runtimes. Async Mono falha no helper ValueTask do EF Core 3.1 e NÃO foi reportado como aprovado.

Linha principal EF Core 10 passou 74 testes unitários com conector 2.6.1. O gate de audit encontrou também XML/Pkcs e SourceLink de build antigos; corrigidos por atualização de pacotes, sem desativar audit. Workflow de publicação passou a selecionar projeto 3.2 ou 10 conforme a versão e manteve audit ativo.

Workflow específico verifica banco real em Linux e carga/modelo em .NET Framework nativo Windows. Resultado remoto e integração EFData/Standard seguem no [plano](../PLAN-NETSTANDARD-CONNECTOR.md). Nenhum deploy ou operação de negócio de produção.
