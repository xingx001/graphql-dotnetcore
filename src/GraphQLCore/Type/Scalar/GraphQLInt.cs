﻿namespace GraphQLCore.Type.Scalar
{
    using Language.AST;
    using System;
    using Translation;
    public class GraphQLInt : GraphQLScalarType
    {
        public GraphQLInt() : base(
            "Int",
            "The `Int` scalar type represents non-fractional signed whole numeric values. Int can represent values between -(2^31) and 2^31 - 1.")
        {
        }

        public override Result GetValueFromAst(GraphQLValue astValue, ISchemaRepository schemaRepository)
        {
            if (astValue.Kind == ASTNodeKind.IntValue)
            {
                decimal value;
                if (!decimal.TryParse(((GraphQLScalarValue)astValue).Value, out value))
                    return Result.Invalid;

                if (value <= int.MaxValue && value >= int.MinValue)
                    return new Result(Convert.ToInt32(value));
            }

            return Result.Invalid;
        }
    }
}