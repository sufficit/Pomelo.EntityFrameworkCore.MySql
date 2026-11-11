# Pomelo de compatibilidade Sufficit

Perfil EF Core 3.1/netstandard2.0 gerado da revisão `520f4dbc7733ca5f97122fdf0dddfcf77e1b6cef`
da linha `3.2-maint` do fork Sufficit. Atualiza o namespace e as APIs do
MySqlConnector para a linha corrigida >=2.6.1. O projeto principal continua EF Core 10.

Build: `dotnet pack compat/netstandard2.0/Pomelo.Compatibility.csproj -c Release`.
O gerador extrai os fontes fixados em `obj`, incluindo recursos, e compila uma DLL
assinada com a chave Pomelo original. O assembly conserva a identidade 3.2.7.0;
o pacote é 3.2.8-sufficit.1 para não substituir o pacote publicado upstream.
Consumidores que dependem do namespace antigo do conector precisam ser recompilados.

[Plano e validação](../../docs/PLAN-NETSTANDARD-CONNECTOR.md).

## Validação

O executável `checks/Checks.csproj` roda no .NET 10 e .NET Framework 4.7.2.
`--offline` verifica identidade, chave, recursos, criação do modelo, seleção do
conector e o helper assíncrono de fechamento de uma conexão já fechada, sem rede. Para CRUD/transações, definir
`SUFFICIT_POMELO_COMPAT_TEST_CONNECTION`: somente servidor 127.0.0.1 e banco
com prefixo `pomelo_compat_test_` são aceitos.

O controle `-p:UseUnpatchedProvider=true` usa o pacote 3.2.7 original com o
conector novo: o probe falha com TypeLoadException, reproduzindo a necessidade
de recompilar o provider. O caminho padrão passa com a DLL do fork.

No Mono instalado, I/O assíncrono do EF Core 3.1 encontra um conflito de
ValueTask em DbConnectionExtensions. O teste local Mono usa
`--synchronous-io` (também ao usar `--offline`); o mesmo CRUD/transações assíncrono passou em .NET 10.
Isso não é reportado como validação de I/O assíncrono em Mono. O workflow
validate-compatibility também verifica carga/modelo no Windows/.NET Framework.
