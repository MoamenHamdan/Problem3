package Sowlutions;

import java.util.Scanner;

public class Problem1 {

		//calculating the GCD
	    public static int gcd(int a, int b) {
	        while (b != 0) {
	            int temp = b;
	            b = a % b;
	            a = temp;
	        }
	        return a;
	    }

	    // LCM using GCD
	    public static int lcm(int a, int b) {
	        return a * (b / gcd(a, b));
	    }

	    // LCM from 1 to n
	    public static int smallestMultiple(int n) {
	        int result = 1;
	        for (int i = 2; i <= n; i++) {
	            result = lcm(result, i);
	        }
	        return result;
	    }

	    public static void main(String[] args) {
	    	Scanner moe = new Scanner (System.in);
	        int n =moe.nextInt();
	        System.out.println("Smallest number divisible by all numbers from 1 to " + n + " is: " + smallestMultiple(n));
	    }
	}
