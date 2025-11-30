using CitizensFinancialGroup.Threvw.Policies.Domain;

namespace Policy.Domain.Tests.Unit {
    [TestClass]
    public class ValueComparerTestFixture {
        private ValueComparer _comparer = null!;

        [TestInitialize]
        public void Setup() {
            _comparer = new ValueComparer();
        }

        #region AreEqual Tests

        [TestMethod]
        public void AreEqual_BothNull_ReturnsTrue() {
            // Act
            var result = _comparer.AreEqual(null!, null!);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreEqual_FirstNull_ReturnsFalse() {
            // Act
            var result = _comparer.AreEqual(null!, "value");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AreEqual_SecondNull_ReturnsFalse() {
            // Act
            var result = _comparer.AreEqual("value", null!);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AreEqual_SameStringValues_ReturnsTrue() {
            // Act
            var result = _comparer.AreEqual("test", "test");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreEqual_DifferentCaseStrings_ReturnsTrue() {
            // Act
            var result = _comparer.AreEqual("Test", "TEST");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreEqual_SameIntegerValues_ReturnsTrue() {
            // Act
            var result = _comparer.AreEqual(42, 42);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreEqual_DifferentIntegerValues_ReturnsFalse() {
            // Act
            var result = _comparer.AreEqual(42, 43);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AreEqual_IntAndDouble_SameValue_ReturnsTrue() {
            // Act
            var result = _comparer.AreEqual(42, 42.0);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreEqual_IntAndDouble_DifferentValue_ReturnsFalse() {
            // Act
            var result = _comparer.AreEqual(42, 42.5);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AreEqual_BooleanValues_Same_ReturnsTrue() {
            // Act
            var result = _comparer.AreEqual(true, true);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreEqual_BooleanValues_Different_ReturnsFalse() {
            // Act
            var result = _comparer.AreEqual(true, false);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Compare Tests

        [TestMethod]
        public void Compare_IntegerValues_FirstLess_ReturnsNegative() {
            // Act
            var result = _comparer.Compare(5, 10);

            // Assert
            Assert.IsTrue(result < 0);
        }

        [TestMethod]
        public void Compare_IntegerValues_FirstGreater_ReturnsPositive() {
            // Act
            var result = _comparer.Compare(10, 5);

            // Assert
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void Compare_IntegerValues_Equal_ReturnsZero() {
            // Act
            var result = _comparer.Compare(10, 10);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Compare_DoubleValues_FirstLess_ReturnsNegative() {
            // Act
            var result = _comparer.Compare(5.5, 10.2);

            // Assert
            Assert.IsTrue(result < 0);
        }

        [TestMethod]
        public void Compare_MixedNumericTypes_ReturnsCorrectComparison() {
            // Act
            var result = _comparer.Compare(42, 42.5);

            // Assert
            Assert.IsTrue(result < 0);
        }

        [TestMethod]
        public void Compare_DateTimeValues_FirstEarlier_ReturnsNegative() {
            // Arrange
            var date1 = new DateTime(2024, 1, 1);
            var date2 = new DateTime(2024, 12, 31);

            // Act
            var result = _comparer.Compare(date1, date2);

            // Assert
            Assert.IsTrue(result < 0);
        }

        [TestMethod]
        public void Compare_DateTimeOffsetValues_FirstEarlier_ReturnsNegative() {
            // Arrange
            var date1 = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var date2 = new DateTimeOffset(2024, 12, 31, 0, 0, 0, TimeSpan.Zero);

            // Act
            var result = _comparer.Compare(date1, date2);

            // Assert
            Assert.IsTrue(result < 0);
        }

        [TestMethod]
        public void Compare_StringValues_FirstLess_ReturnsNegative() {
            // Act
            var result = _comparer.Compare("apple", "banana");

            // Assert
            Assert.IsTrue(result < 0);
        }

        [TestMethod]
        public void Compare_StringValues_CaseInsensitive_ReturnsCorrectComparison() {
            // Act
            var result = _comparer.Compare("Apple", "apple");

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Compare_FirstNull_ThrowsArgumentNullException() {
            // Act
            _comparer.Compare(null!, "value");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Compare_SecondNull_ThrowsArgumentNullException() {
            // Act
            _comparer.Compare("value", null!);
        }

        #endregion
    }
}
