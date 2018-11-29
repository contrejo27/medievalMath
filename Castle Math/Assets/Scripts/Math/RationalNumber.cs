using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Rational {
    public int numerator { get; set; }
    public int denominator { get; set; }

    public Rational () {
    }

    public Rational (int numerator, int denominator) {
        // TODO: Numerator is the only one that can be negative
        // TODO: Denominator cannot be zero
    }

    public void Simplify() {
        int factor = GCD(this.numerator, this.denominator);
        this.numerator = (int) this.numerator / factor;
        this.denoninator = (int) this.denominator / factor;
    }

    public bool IsSimplifiable() {
        if ( (this.numerator % this.denominator == 0) || (this.denominator % this.numerator  == 0) ) {
            return true;
        }
        return false;
    }

    public double Decimal {
        /* set { _decimal = value; } */
        get { return (double) this.numerator/this.denominator; }
    }

    public string FractionString {
        get { return string.Format("{0}/{1}", this.numerator, this.denominator); }
    }

    public Rational Add (Rational other) {
        new_denominator = LCM(this.denominator, other.denominator);
        new_numerator = this.numerator*new_denominator/this.denominator + other.numerator*new_denominator/other.denominator;
        
        Rational new_fraction = new Rational(new_numerator, new_denominator);
        new_fraction.Simplify();
        
        return new_fraction;
    }

    public Rational Subtract (Rational other) {
        // TODO: Implement using Add()
    }

}


public int[] PrimesBelow (int n) {
    /// <summary>
    /// Lists primes less than n
    /// </summary>

    bool[] sieve = Enumerable.Repeat(true, n).ToArray();

    for (int i = 3, i < Math.Pow(n,0.5)+1, i += 2) {
        
    }

    // Python Code: Tested
    // sieve = [True] * n
    // for i in range(3, int(n**0.5)+1, 2):
    //     if sieve[i]:
    //         sieve[i*i::2*i] = [False] * ((n - i*i - 1)//(2*i) + 1)
    // return [2] + [i for i in range(3, n, 2) if sieve[i]]
}


public int GCD(int a, int b) {
    /// <summary>
    /// Greatest Common Divisor
    /// 
    /// Using Euclid's Algorithm
    /// 
    /// while b:
    ///     a, b = b, a%b
    /// return a
    /// </summary>

    while (b) { 
        a = b;
        b = a%b;
    }
    
    return a
}


public int LCM (int a, int b) {
    /// <summary>
    /// Least Common Multiple
    /// </summary>

    return (int) (a * b / GCD(a, b))
}
