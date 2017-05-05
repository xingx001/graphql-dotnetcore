﻿namespace GraphQLCore.Tests.Validation
{
    using NUnit.Framework;
    using Exceptions;
    using GraphQLCore.Validation.Rules;
    using System.Linq;

    [TestFixture]
    public class FieldsOnCorrectTypeTests : ValidationTestBase
    {
        [Test]
        public void ObjectFieldSelection_ReportsNoError()
        {
            var errors = Validate(@"
            fragment objectFieldSelection on SimpleObjectType {
                __typename
                booleanField
            }
            ");

            Assert.IsEmpty(errors);
        }

        [Test]
        public void AliasedObjectFieldSelection_ReportsNoError()
        {
            var errors = Validate(@"
            fragment aliasedObjectFieldSelection on SimpleObjectType {
                tn : __typename
                otherBooleanField : booleanField
            }
            ");

            Assert.IsEmpty(errors);
        }

        [Test]
        public void InterfaceFieldSelection_ReportsNoError()
        {
            var errors = Validate(@"
            fragment interfaceFieldSelection on SimpleInterfaceType {
                __typename
                booleanField
            }    
            ");

            Assert.IsEmpty(errors);
        }

        [Test]
        public void AliasedInterfaceFieldSelection_ReportsNoError()
        {
            var errors = Validate(@"
            fragment interfaceFieldSelection on SimpleInterfaceType {
                otherBooleanField : booleanField
            }
            ");

            Assert.IsEmpty(errors);
        }

        [Test]
        public void LyingAliasSelection_ReportsNoError()
        {
            var errors = Validate(@"
            fragment lyingAliasSelection on AnotherSimpleObjectType {
                booleanField : boolField
            }
            ");

            Assert.IsEmpty(errors);
        }

        [Test]
        public void FieldsOnUnknownType_ReportsNoError()
        {
            var errors = Validate(@"
            fragment unknownSelection on UnknownType {
                unknownField
            }
            ");

            Assert.IsEmpty(errors);
        }

        [Test]
        public void FieldsOnNestedKnownType_ReportsSingleError()
        {
            var errors = Validate(@"
            fragment typeKnownAgain on SimpleInterfaceType {
                ... on SimpleObjectType {
                    unknown_object_field
                }
            }
            ");

            Assert.AreEqual("Cannot query field \"unknown_object_field\" on type \"SimpleObjectType\".", errors.Single().Message);
        }

        [Test]
        public void FieldNotDefinedOnFragment_ReportsSingleError()
        {
            var errors = Validate(@"
            fragment fieldNotDefined on AnotherSimpleObjectType {
                simple
            }
            ");

            Assert.AreEqual("Cannot query field \"simple\" on type \"AnotherSimpleObjectType\". Did you mean \"sample\"?", errors.Single().Message);
        }

        [Test]
        public void DeeplyUnknownField_ReportsNoErrorForDeepField()
        {
            var errors = Validate(@"
            fragment deepFieldNotDefined on SimpleObjectType {
                unknown_field {
                    deeper_unknown_field
                }
            }
            ");

            Assert.AreEqual("Cannot query field \"unknown_field\" on type \"SimpleObjectType\".", errors.Single().Message);
        }

        [Test]
        public void SubfieldNotDefined_ReportsSingleError()
        {
            var errors = Validate(@"
            fragment subFieldNotDefined on ComplicatedObjectType {
                complicatedObjectArray {
                    unknown_field
                }
            }
            ");

            Assert.AreEqual("Cannot query field \"unknown_field\" on type \"ComplicatedObjectType\".", errors.Single().Message);
        }

        [Test]
        public void FieldNotDefinedOnInlineFragment_ReportsSingleError()
        {
            var errors = Validate(@"
            fragment fieldNotDefined on SimpleInterfaceType {
                ... on SimpleObjectType {
                    sample
                }
            }
            ");

            Assert.AreEqual("Cannot query field \"sample\" on type \"SimpleObjectType\". Did you mean \"simple\"?", errors.Single().Message);
        }

        [Test]
        public void AliasedFieldTargetNotDefined_ReportsSingleError()
        {
            var errors = Validate(@"
            fragment aliasedFieldTargetNotDefined on SimpleObjectType {
                simpleSample : sample
            }
            ");

            Assert.AreEqual("Cannot query field \"sample\" on type \"SimpleObjectType\". Did you mean \"simple\"?", errors.Single().Message);
        }

        [Test]
        public void AliasedLyingFieldTargetNotDefined_ReportsSingleError()
        {
            var errors = Validate(@"
            fragment aliasedFieldTargetNotDefined on SimpleObjectType {
                simple : sample
            }
            ");

            Assert.AreEqual("Cannot query field \"sample\" on type \"SimpleObjectType\". Did you mean \"simple\"?", errors.Single().Message);
        }

        [Test]
        public void NotDefinedOnInterface_ReportsSingleError()
        {
            var errors = Validate(@"
            fragment notDefinedOnInterface on SimpleInterfaceType {
                unknownInterfaceField
            }
            ");

            Assert.AreEqual("Cannot query field \"unknownInterfaceField\" on type \"SimpleInterfaceType\".", errors.Single().Message);
        }

        [Test]
        public void DefinedOnImplementors_ReportsSingleError()
        {
            var errors = Validate(@"
            fragment notDefinedOnInterface on SimpleInterfaceType {
                notInterfaceField
            }
            ");

            Assert.AreEqual("Cannot query field \"notInterfaceField\" on type \"SimpleInterfaceType\". Did you mean to use an inline fragment on \"SimpleObjectType\" or \"AnotherSimpleObjectType\"?", errors.Single().Message);
        }

        [Test]
        public void MetaFieldSelectionOnUnion_ReportsNoError()
        {
            var errors = Validate(@"
            fragment directFieldSelectionOnUnion on SimpleSampleUnionType {
                __typename
            }
            ");

            Assert.IsEmpty(errors);
        }

        [Test]
        public void DirectFieldSelectionOnUnion_ReportsNoError()
        {
            var errors = Validate(@"
            fragment directFieldSelectionOnUnion on SimpleSampleUnionType {
                directField
            }
            ");

            Assert.AreEqual("Cannot query field \"directField\" on type \"SimpleSampleUnionType\".", errors.Single().Message);
        }

        [Test]
        public void DefinedOnImplementorsQueriedOnUnion_ReportsSingleError()
        {
            var errors = Validate(@"
            fragment definedOnImplementorsQueriedOnUnion on SimpleSampleUnionType {
                booleanField
            }
            ");

            Assert.AreEqual("Cannot query field \"booleanField\" on type \"SimpleSampleUnionType\". Did you mean to use an inline fragment on \"SimpleInterfaceType\", \"SimpleObjectType\", or \"AnotherSimpleObjectType\"?", errors.Single().Message);
        }

        [Test]
        public void ValidFieldInInlineFragment_ReportsNoError()
        {
            var errors = Validate(@"
            fragment objectFieldSelection on SimpleInterfaceType {
                ... on SimpleObjectType {
                    booleanField
                }
                ... {
                    booleanField
                }
            }
            ");

            Assert.IsEmpty(errors);
        }

        protected override GraphQLException[] Validate(string body)
        {
            return validationContext.Validate(
                GetAst(body),
                this.validationTestSchema,
                new IValidationRule[]
                {
                    new FieldsOnCorrectType()
                });
        }
    }
}
