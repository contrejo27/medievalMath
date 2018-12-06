using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rational {
    public int num { get; set; }
    public int den { get; set; }

    public Rational () {
    }

    public Rational (int num, int den) {
        // TODO: Numerator is the only one that can be negative
        this.num = num;

        // TODO: Denominator cannot be zero
        this.den = den;
    }

    public void Simplify () {
        int factor = GCD(this.num, this.den);
        this.num = (int) this.num / factor;
        this.den = (int) this.den / factor;
    }

    public bool IsSimplifiable () {
        // TODO: Implement the correct IsSimplifiable()
        if ( (this.num % this.den) == 0 ) {
            return true;
        }
        else if ( (this.den % this.num) == 0 ) {
            return true;
        }
        else {
            return false;
        }
    }

    public double Decimal {
        /* set { _decimal = value; } */
        get { return (double) this.num/this.den; }
    }

    public static Rational operator + (Rational a, Rational b) {
        int new_den = LCM(a.den, b.den);
        int new_num = a.num*new_den/a.den + b.num*new_den/b.den;

        Rational result = new Rational(new_num, new_den);
        result.Simplify();

        return result;
    }

    public static Rational operator - (Rational a, Rational b) {
        // TODO: Implement using operator +
        return new Rational();
    }

    public static Rational operator * (Rational a, Rational b) {
        Rational result = new Rational(a.num * b.num, a.den * b.den);
        result.Simplify();

        return result;
    }

    public static Rational operator / (Rational a, Rational b) {
        // TODO: Implement using operator *
        return new Rational();
    }

    public static bool operator < (Rational a, Rational b) {
        // TODO: Implement 
        bool status = false;
        return status;
    }

    public static bool operator <= (Rational a, Rational b) {
        // TODO: Implement 
        bool status = false;
        return status;
    }

    public static bool operator > (Rational a, Rational b) {
        // TODO: Implement 
        bool status = false;
        return status;
    }

    public static bool operator >= (Rational a, Rational b) {
        // TODO: Implement 
        bool status = false;
        return status;
    }

    public static bool operator == (Rational a, Rational b) {
        // TODO: Implement 
        bool status = false;
        return status;
    }

    public static bool operator != (Rational a, Rational b) {
        // TODO: Implement 
        bool status = false;
        return status;
    }

    public static implicit operator double (Rational f) {
        return (double) f.num / f.den;
    }

    public override string ToString () {
        return string.Format("{0}/{1}", this.num, this.den);
    }

    // MATH TOOLS
    public int[] PrimesBelow (int n) {
        /// <summary>
        /// Lists primes less than n
        /// </summary>

        List<int> primes = GeneratePrimesSieveOfSundaram(n);

        return primes.ToArray();
    }

    public static BitArray SieveOfSundaram(int limit) {
        limit /= 2;
        BitArray bits = new BitArray(limit + 1, true);
        for (int i = 1; 3 * i + 1 < limit; i++) {
            for (int j = 1; i + j + 2 * i * j <= limit; j++) {
                bits[i + j + 2 * i * j] = false;
            }
        }
        return bits;
    }

    public static List<int> GeneratePrimesSieveOfSundaram(int n) {
        // int limit = ApproximateNthPrime(n);
        int limit = n;
        BitArray bits = SieveOfSundaram(limit);
        List<int> primes = new List<int>();
        primes.Add(2);
        for (int i = 1, found = 1; 2 * i + 1 <= limit && found < n; i++) {
            if (bits[i]) {
                primes.Add(2 * i + 1);
                found++;
            }
        }
        return primes;
    }

    public static int GCD(int a, int b) {
        /// <summary>
        /// Greatest Common Divisor
        /// 
        /// Using Euclid's Algorithm
        /// 
        /// while b:
        ///     a, b = b, a%b
        /// return a
        /// </summary>

        while (b != 0) { 
            a = b;
            b = a%b;
        }

        return a;
    }

    public static int LCM (int a, int b) {
        /// <summary>
        /// Least Common Multiple
        /// </summary>

        return (int) (a * b / GCD(a, b));
    }

}

class Tester {
    static Rational a = new Rational(2, 3);
    static Rational b = new Rational(6, 3);
    static Rational c = new Rational(12, 6);
    static Rational d = new Rational(-7, 3);

    static void Main(string[] args) {

        TestStringConversion();
        TestSimplification();
        // TODO: Test Decimal
        // TODO: Test Addition
        // TODO: Test Subtraction
        // TODO: Test Multiplication
        // TODO: Test Division
        // TODO: Test ==
        // TODO: Test <
        // TODO: Test <=
        // TODO: Test >
        // TODO: Test >=
        // TODO: Test !=

    }

    static void TestStringConversion () {
        if ( a.ToString() != "2/3" ) {
            Console.WriteLine("Failed Test: String Conversion (1)");
        }
        if ( d.ToString() != "-7/3" ) {
            Console.WriteLine("Failed Test: String Conversion (2)");
        }
    }

    static void TestSimplification () {
        if ( a.IsSimplifiable() != false ) {
            Console.WriteLine("Failed Test: Simplificationn (1)");
        }

        if ( b.IsSimplifiable() != true ) {
            Console.WriteLine("Failed Test: Simplificationn (2)");
        }
    }

    static void TestDecimal () {
        if ( b.IsSimplifiable() != true ) {
            Console.WriteLine("Failed Test: Simplificationn (2)");
        }
    }

} // Tester Class
