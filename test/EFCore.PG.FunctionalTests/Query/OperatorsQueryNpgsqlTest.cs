using Microsoft.EntityFrameworkCore.TestModels.Operators;
using Npgsql.EntityFrameworkCore.PostgreSQL.TestUtilities;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.Query;

public class OperatorsQueryNpgsqlTest : OperatorsQueryTestBase
{
    public OperatorsQueryNpgsqlTest(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    protected override ITestStoreFactory TestStoreFactory
        => NpgsqlTestStoreFactory.Instance;

    protected TestSqlLoggerFactory TestSqlLoggerFactory
        => (TestSqlLoggerFactory)ListLoggerFactory;

    protected void AssertSql(params string[] expected)
        => TestSqlLoggerFactory.AssertBaseline(expected);

    public override async Task Bitwise_and_on_expression_with_like_and_null_check_being_compared_to_false()
    {
        await base.Bitwise_and_on_expression_with_like_and_null_check_being_compared_to_false();

        AssertSql("");
    }

    public override async Task Complex_predicate_with_bitwise_and_modulo_and_negation()
    {
        await base.Complex_predicate_with_bitwise_and_modulo_and_negation();

        AssertSql("");
    }

    public override async Task Complex_predicate_with_bitwise_and_arithmetic_operations()
    {
        await base.Complex_predicate_with_bitwise_and_arithmetic_operations();

        AssertSql("");
    }

    public override async Task Or_on_two_nested_binaries_and_another_simple_comparison()
    {
        await base.Or_on_two_nested_binaries_and_another_simple_comparison();

        AssertSql(
"""
SELECT o."Id" AS "Id1", o0."Id" AS "Id2", o1."Id" AS "Id3", o2."Id" AS "Id4", o3."Id" AS "Id5"
FROM "OperatorEntityString" AS o
CROSS JOIN "OperatorEntityString" AS o0
CROSS JOIN "OperatorEntityString" AS o1
CROSS JOIN "OperatorEntityString" AS o2
CROSS JOIN "OperatorEntityInt" AS o3
WHERE ((((o."Value" = 'A') AND o."Value" IS NOT NULL) AND ((o0."Value" = 'A') AND o0."Value" IS NOT NULL)) OR (((o1."Value" = 'B') AND o1."Value" IS NOT NULL) AND ((o2."Value" = 'B') AND o2."Value" IS NOT NULL))) AND (o3."Value" = 2)
ORDER BY o."Id" NULLS FIRST, o0."Id" NULLS FIRST, o1."Id" NULLS FIRST, o2."Id" NULLS FIRST, o3."Id" NULLS FIRST
""");
    }

    [ConditionalFact(Skip = "#2217")]
    public override async Task Projection_with_not_and_negation_on_integer()
    {
        await base.Projection_with_not_and_negation_on_integer();

        AssertSql(
"""
SELECT ~(-(-([o1].[Value] + [o].[Value] + CAST(2 AS bigint)))) % (-([o0].[Value] + [o0].[Value]) - [o].[Value])
FROM [OperatorEntityLong] AS [o]
CROSS JOIN [OperatorEntityLong] AS [o0]
CROSS JOIN [OperatorEntityLong] AS [o1]
ORDER BY [o].[Id], [o0].[Id], [o1].[Id]
""");
    }

    public override async Task Negate_on_column(bool async)
    {
        await base.Negate_on_column(async);

        AssertSql(
"""
SELECT o."Id"
FROM "OperatorEntityInt" AS o
WHERE o."Id" = -o."Value"
""");
    }

    public override async Task Double_negate_on_column()
    {
        await base.Double_negate_on_column();

        AssertSql(
"""
SELECT o."Id"
FROM "OperatorEntityInt" AS o
WHERE -(-o."Value") = o."Value"
""");
    }

    public override async Task Negate_on_binary_expression(bool async)
    {
        await base.Negate_on_binary_expression(async);

        AssertSql(
"""
SELECT o."Id" AS "Id1", o0."Id" AS "Id2"
FROM "OperatorEntityInt" AS o
CROSS JOIN "OperatorEntityInt" AS o0
WHERE -o."Value" = -(o."Id" + o0."Value")
""");
    }

    public override async Task Negate_on_like_expression(bool async)
    {
        await base.Negate_on_like_expression(async);

        AssertSql(
"""
SELECT o."Id"
FROM "OperatorEntityString" AS o
WHERE o."Value" IS NOT NULL AND NOT (o."Value" LIKE 'A%')
""");
    }

//     [ConditionalTheory]
//     [MemberData(nameof(IsAsyncData))]
//     public virtual async Task Where_AtTimeZone_datetimeoffset_constant(bool async)
//     {
//         var contextFactory = await InitializeAsync<OperatorsContext>(seed: Seed);
//         using var context = contextFactory.CreateContext();
//
//         var expected = (from e in ExpectedData.OperatorEntitiesDateTimeOffset
//                         where e.Value.UtcDateTime == new DateTimeOffset(2000, 1, 1, 18, 0, 0, TimeSpan.Zero)
//                         select e.Id).ToList();
//
//         var actual = (from e in context.Set<OperatorEntityDateTimeOffset>()
//                       where EF.Functions.AtTimeZone(e.Value, "UTC") == new DateTimeOffset(2000, 1, 1, 18, 0, 0, TimeSpan.Zero)
//                       select e.Id).ToList();
//
//         Assert.Equal(expected.Count, actual.Count);
//         for (var i = 0; i < expected.Count; i++)
//         {
//             Assert.Equal(expected[i], actual[i]);
//         }
//
//         AssertSql(
// """
// SELECT [o].[Id]
// FROM [OperatorEntityDateTimeOffset] AS [o]
// WHERE ([o].[Value] AT TIME ZONE 'UTC') = '2000-01-01T18:00:00.0000000+00:00'
// """);
//     }
//
//     [ConditionalTheory]
//     [MemberData(nameof(IsAsyncData))]
//     public virtual async Task Where_AtTimeZone_datetimeoffset_parameter(bool async)
//     {
//         var contextFactory = await InitializeAsync<OperatorsContext>(seed: Seed);
//         using var context = contextFactory.CreateContext();
//
//         var dateTime = new DateTimeOffset(2000, 1, 1, 18, 0, 0, TimeSpan.Zero);
//         var timeZone = "UTC";
//
//         var expected = (from e in ExpectedData.OperatorEntitiesDateTimeOffset
//                         where e.Value.UtcDateTime == dateTime
//                         select e.Id).ToList();
//
//         var actual = (from e in context.Set<OperatorEntityDateTimeOffset>()
//                       where EF.Functions.AtTimeZone(e.Value, timeZone) == dateTime
//                       select e.Id).ToList();
//
//         Assert.Equal(expected.Count, actual.Count);
//         for (var i = 0; i < expected.Count; i++)
//         {
//             Assert.Equal(expected[i], actual[i]);
//         }
//
//         AssertSql(
// """
// @__timeZone_1='UTC' (Size = 8000) (DbType = AnsiString)
// @__dateTime_2='2000-01-01T18:00:00.0000000+00:00'
//
// SELECT [o].[Id]
// FROM [OperatorEntityDateTimeOffset] AS [o]
// WHERE ([o].[Value] AT TIME ZONE @__timeZone_1) = @__dateTime_2
// """);
//     }
//
//     [ConditionalTheory]
//     [MemberData(nameof(IsAsyncData))]
//     public virtual async Task Where_AtTimeZone_datetimeoffset_column(bool async)
//     {
//         var contextFactory = await InitializeAsync<OperatorsContext>(seed: Seed);
//         using var context = contextFactory.CreateContext();
//
//         var expected = (from e1 in ExpectedData.OperatorEntitiesDateTimeOffset
//                         from e2 in ExpectedData.OperatorEntitiesDateTimeOffset
//                         where e1.Value == e2.Value.UtcDateTime
//                         select new { Id1 = e1.Id, Id2 = e2.Id }).ToList();
//
//         var actual = (from e1 in context.Set<OperatorEntityDateTimeOffset>()
//                       from e2 in context.Set<OperatorEntityDateTimeOffset>()
//                       where EF.Functions.AtTimeZone(e1.Value, "UTC") == e2.Value
//                       select new { Id1 = e1.Id, Id2 = e2.Id }).ToList();
//
//         Assert.Equal(expected.Count, actual.Count);
//         for (var i = 0; i < expected.Count; i++)
//         {
//             Assert.Equal(expected[i].Id1, actual[i].Id1);
//             Assert.Equal(expected[i].Id2, actual[i].Id2);
//         }
//
//         AssertSql(
// """
// SELECT [o].[Id] AS [Id1], [o0].[Id] AS [Id2]
// FROM [OperatorEntityDateTimeOffset] AS [o]
// CROSS JOIN [OperatorEntityDateTimeOffset] AS [o0]
// WHERE ([o].[Value] AT TIME ZONE 'UTC') = [o0].[Value]
// """);
    // }

    protected override void Seed(OperatorsContext ctx)
    {
        ctx.Set<OperatorEntityString>().AddRange(ExpectedData.OperatorEntitiesString);
        ctx.Set<OperatorEntityInt>().AddRange(ExpectedData.OperatorEntitiesInt);
        ctx.Set<OperatorEntityNullableInt>().AddRange(ExpectedData.OperatorEntitiesNullableInt);
        ctx.Set<OperatorEntityLong>().AddRange(ExpectedData.OperatorEntitiesLong);
        ctx.Set<OperatorEntityBool>().AddRange(ExpectedData.OperatorEntitiesBool);
        ctx.Set<OperatorEntityNullableBool>().AddRange(ExpectedData.OperatorEntitiesNullableBool);
        // ctx.Set<OperatorEntityDateTimeOffset>().AddRange(ExpectedData.OperatorEntitiesDateTimeOffset);

        ctx.SaveChanges();
    }
}
