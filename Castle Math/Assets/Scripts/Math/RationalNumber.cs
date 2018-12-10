using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Rational {
    public int num { get; set; }
    public int den { get; set; }

    

    public Rational () {
    }

    public Rational (int num, int den) {
        this.num = num;

        if(den == 0)
        {
            this.den = 1;
            Debug.LogError("Rational Number denominator is 0. Can't divide by 0.");
        } else if(den < 0)
        {
            this.den = den * -1;
            this.num = num * -1;

        } else
        {
            this.den = den;
        }
    }

    public void Simplify () {
        int factor = GCD(this.num, this.den);
        this.num = (int) this.num / factor;
        this.den = (int) this.den / factor;
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
        int new_den = LCM(a.den, b.den);
        int new_num = a.num * new_den / a.den + -1 * (b.num * new_den / b.den);

        Rational result = new Rational(new_num, new_den);
        result.Simplify();
        return result;
    }

    public static Rational operator * (Rational a, Rational b) {
        Rational result = new Rational(a.num * b.num, a.den * b.den);
        result.Simplify();

        return result;
    }

    public static Rational operator / (Rational a, Rational b) {
        Rational zeroCase;
        if (b.num == 0 || a.num == 0)
        {
            zeroCase = new Rational(0, 1);
            return zeroCase;
        }
        Rational result = new Rational(a.num * b.den, a.den * b.num);
        result.Simplify();

        return result;
    }

    public static bool operator < (Rational a, Rational b) {
        bool status = false;
        float num1 = a.num / (float)a.den;
        float num2 = b.num / (float)b.den;
        if (num1 < num2)
            status = true;
        
        return status;
    }

    public static bool operator <= (Rational a, Rational b) {
        bool status = false;
        float num1 = a.num / (float)a.den;
        float num2 = b.num / (float)b.den;
        if (num1 <= num2)
            status = true;

        return status;
    }

    public static bool operator > (Rational a, Rational b) {
        bool status = false;
        float num1 = a.num / (float)a.den;
        float num2 = b.num / (float)b.den;
        if (num1 > num2)
            status = true;

        return status;
    }

    public static bool operator >= (Rational a, Rational b) {
        bool status = false;
        float num1 = a.num / (float)a.den;
        float num2 = b.num / (float)b.den;
        if (num1 >= num2)
            status = true;

        return status;
    }

    public static bool operator == (Rational a, Rational b) {
        bool status = false;
        float num1 = a.num / (float)a.den;
        float num2 = b.num / (float)b.den;
        if (num1 == num2)
            status = true;

        return status;
    }

    public static bool operator != (Rational a, Rational b) {
        bool status = false;
        float num1 = a.num / (float)a.den;
        float num2 = b.num / (float)b.den;
        if (num1 != num2)
            status = true;

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
        if(b == 0)
        {
            return 1;
        }

        int rem = 1;

        if (a < b) {
            int temp = b;
            b = a;
            a = temp;
        }
        
        while (rem != 0) {
            rem = a % b;
            a = b;
            if (rem == 0)
            {
                break;
            }
            b = rem;
        }
        if(b == 0)
        {
            return 1;
        }
        return b;
    }

    public static int LCM (int a, int b) {
        /// <summary>
        /// Least Common Multiple
        /// </summary>

        return (int) (a * b / GCD(a, b));
    }

}

public class Tester {
    public static Rational a = new Rational(Random.Range(0, 10), Random.Range(0, 10));
    public static Rational b = new Rational(Random.Range(10, 100), Random.Range(10, 100));
    public static Rational c = new Rational(Random.Range(-10, 10), Random.Range(-10, 10));
    public static Rational d = new Rational(Random.Range(0, 10), Random.Range(0, 10));

    static void Main(string[] args) {

        //TestStringConversion();
        //TestSimplification();
        // TODO: Test Decimal

    }

    static void TestStringConversion() {
        if (a.ToString() != "2/3") {
            Console.WriteLine("Failed Test: String Conversion (1)");
        }
        if (d.ToString() != "-7/3") {
            Console.WriteLine("Failed Test: String Conversion (2)");
        }
    }
    /*
    static void TestSimplification() {
        if (a.IsSimplifiable() != false) {
            Console.WriteLine("Failed Test: Simplificationn (1)");
        }

        if (b.IsSimplifiable() != true) {
            Console.WriteLine("Failed Test: Simplificationn (2)");
        }
    }
    
    static void TestDecimal () {
        if ( b.IsSimplifiable() != true ) {
            Console.WriteLine("Failed Test: Simplificationn (2)");
        }
    }
    
    static void TestDecimal()
    {
        if (b.IsSimplifiable() != true)
        {
            Console.WriteLine("Failed Test: Simplificationn (2)");
        }
    }
    */

    public static void RandomizeTestVar()
    {
        Rational testvar1 = new Rational(Random.Range(-10, 10), Random.Range(-10, 10));
        Rational testvar2 = new Rational(Random.Range(-10, 10), Random.Range(-10, 10));
        //Rational testvar1 = new Rational(5, 6);
        //Rational testvar2 = new Rational(5, 6);
        bool result;
        if (testvar1.den == 0)
        {
            testvar1 = new Rational(Random.Range(-10, 10), Random.Range(-10, 10));
        }
        if (testvar2.den == 0)
        {
            testvar2 = new Rational(Random.Range(-10, 10), Random.Range(-10, 10));
        }

        Debug.Log("Before: (" + testvar1 + ") <= (" + testvar2 + ")");
        result = testvar1 <= testvar2;
        Debug.Log("After: = " + result);
        
    }

}

 // Tester Class
