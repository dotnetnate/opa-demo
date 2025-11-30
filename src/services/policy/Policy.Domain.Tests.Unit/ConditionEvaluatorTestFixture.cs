using CitizensFinancialGroup.Threvw.Policies.Domain;
using Microsoft.Extensions.Logging;
using Moq;

namespace Policy.Domain.Tests.Unit {
    [TestClass]
    public class ConditionEvaluatorTestFixture {
        private Mock<ILogger<ConditionEvaluator>> _mockLogger = null!;
        private Mock<IValueComparer> _mockComparer = null!;
        private ConditionEvaluator _evaluator = null!;

        [TestInitialize]
        public void Setup() {
            _mockLogger = new Mock<ILogger<ConditionEvaluator>>();
            _mockComparer = new Mock<IValueComparer>();
            _evaluator = new ConditionEvaluator(_mockLogger.Object, _mockComparer.Object);
        }

        [TestMethod]
        public void EvaluateConditions_NullConditions_ReturnsTrue() {
            // Arrange
            var context = new Dictionary<string, object>();

            // Act
            var result = _evaluator.EvaluateConditions(null!, context);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateConditions_EmptyConditions_ReturnsTrue() {
            // Arrange
            var conditions = new List<Condition>();
            var context = new Dictionary<string, object>();

            // Act
            var result = _evaluator.EvaluateConditions(conditions, context);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateConditions_SingleCondition_Equals_Match_ReturnsTrue() {
            // Arrange
            var conditions = new List<Condition> {
                new Condition {
                    ContextAttributePath = "amount",
                    Operator = Operators.eq,
                    Value = 1000
                }
            };
            var context = new Dictionary<string, object> {
                { "amount", 1000 }
            };

            _mockComparer.Setup(c => c.AreEqual(1000, 1000)).Returns(true);

            // Act
            var result = _evaluator.EvaluateConditions(conditions, context);

            // Assert
            Assert.IsTrue(result);
            _mockComparer.Verify(c => c.AreEqual(1000, 1000), Times.Once);
        }

        [TestMethod]
        public void EvaluateConditions_SingleCondition_Equals_NoMatch_ReturnsFalse() {
            // Arrange
            var conditions = new List<Condition> {
                new Condition {
                    ContextAttributePath = "amount",
                    Operator = Operators.eq,
                    Value = 1000
                }
            };
            var context = new Dictionary<string, object> {
                { "amount", 2000 }
            };

            _mockComparer.Setup(c => c.AreEqual(2000, 1000)).Returns(false);

            // Act
            var result = _evaluator.EvaluateConditions(conditions, context);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateConditions_AttributeNotInContext_ReturnsFalse() {
            // Arrange
            var conditions = new List<Condition> {
                new Condition {
                    ContextAttributePath = "missing",
                    Operator = Operators.eq,
                    Value = 1000
                }
            };
            var context = new Dictionary<string, object>();

            // Act
            var result = _evaluator.EvaluateConditions(conditions, context);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateConditions_MultipleConditions_AllMatch_ReturnsTrue() {
            // Arrange
            var conditions = new List<Condition> {
                new Condition {
                    ContextAttributePath = "amount",
                    Operator = Operators.gt,
                    Value = 100
                },
                new Condition {
                    ContextAttributePath = "status",
                    Operator = Operators.eq,
                    Value = "active"
                }
            };
            var context = new Dictionary<string, object> {
                { "amount", 500 },
                { "status", "active" }
            };

            _mockComparer.Setup(c => c.Compare(500, 100)).Returns(1);
            _mockComparer.Setup(c => c.AreEqual("active", "active")).Returns(true);

            // Act
            var result = _evaluator.EvaluateConditions(conditions, context);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateConditions_MultipleConditions_OneDoesNotMatch_ReturnsFalse() {
            // Arrange
            var conditions = new List<Condition> {
                new Condition {
                    ContextAttributePath = "amount",
                    Operator = Operators.gt,
                    Value = 100
                },
                new Condition {
                    ContextAttributePath = "status",
                    Operator = Operators.eq,
                    Value = "active"
                }
            };
            var context = new Dictionary<string, object> {
                { "amount", 500 },
                { "status", "inactive" }
            };

            _mockComparer.Setup(c => c.Compare(500, 100)).Returns(1);
            _mockComparer.Setup(c => c.AreEqual("inactive", "active")).Returns(false);

            // Act
            var result = _evaluator.EvaluateConditions(conditions, context);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateConditions_NotEquals_Match_ReturnsTrue() {
            // Arrange
            var conditions = new List<Condition> {
                new Condition {
                    ContextAttributePath = "status",
                    Operator = Operators.neq,
                    Value = "inactive"
                }
            };
            var context = new Dictionary<string, object> {
                { "status", "active" }
            };

            _mockComparer.Setup(c => c.AreEqual("active", "inactive")).Returns(false);

            // Act
            var result = _evaluator.EvaluateConditions(conditions, context);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateConditions_GreaterThanOrEqual_Match_ReturnsTrue() {
            // Arrange
            var conditions = new List<Condition> {
                new Condition {
                    ContextAttributePath = "amount",
                    Operator = Operators.gte,
                    Value = 1000
                }
            };
            var context = new Dictionary<string, object> {
                { "amount", 1000 }
            };

            _mockComparer.Setup(c => c.Compare(1000, 1000)).Returns(0);

            // Act
            var result = _evaluator.EvaluateConditions(conditions, context);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateConditions_LessThan_Match_ReturnsTrue() {
            // Arrange
            var conditions = new List<Condition> {
                new Condition {
                    ContextAttributePath = "amount",
                    Operator = Operators.lt,
                    Value = 1000
                }
            };
            var context = new Dictionary<string, object> {
                { "amount", 500 }
            };

            _mockComparer.Setup(c => c.Compare(500, 1000)).Returns(-1);

            // Act
            var result = _evaluator.EvaluateConditions(conditions, context);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateConditions_LessThanOrEqual_Match_ReturnsTrue() {
            // Arrange
            var conditions = new List<Condition> {
                new Condition {
                    ContextAttributePath = "amount",
                    Operator = Operators.lte,
                    Value = 1000
                }
            };
            var context = new Dictionary<string, object> {
                { "amount", 1000 }
            };

            _mockComparer.Setup(c => c.Compare(1000, 1000)).Returns(0);

            // Act
            var result = _evaluator.EvaluateConditions(conditions, context);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
