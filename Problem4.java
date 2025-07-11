package Sowlutions;
import java.nio.file.*;
import java.util.*;
public class Problem4 {

	public static void main(String[] args)throws Exception {
		   //Load the encrypted ASCII codes
		        String content = Files.readString(Path.of("C:\\Users\\Admin\\Desktop\\p059_cipher.txt"));
		        String[] split = content.split(",");
		        int[] encrypted = Arrays.stream(split).mapToInt(Integer::parseInt).toArray();

		        int maxSum = 0;
		        String decryptedText = "";
		        String bestKey = "";

		        //Try all lowercase - letter combinations
		        for (char a = 'a'; a <= 'z'; a++) {
		            for (char b = 'a'; b <= 'z'; b++) {
		                for (char c = 'a'; c <= 'z'; c++) {
		                    char[] key = {a, b, c};
		                    StringBuilder sb = new StringBuilder();
		                    int sum = 0;

		                    for (int i = 0; i < encrypted.length; i++) {
		                        char decryptedChar = (char) (encrypted[i] ^ key[i % 3]);
		                        sb.append(decryptedChar);
		                        sum += decryptedChar;
		                    }

		                    String text = sb.toString();

		                    //Simple English detection 
		                    if (text.contains(" the ") && text.contains(" and ") && text.contains("is")) {
		                        if (sum > maxSum) {
		                            maxSum = sum;
		                            decryptedText = text;
		                            bestKey = "" + a + b + c;
		                        }
		                    }
		                }
		            }
		        }
		      
		        //Output result
		        System.out.println("Password: " + bestKey);
		        System.out.println("Decrypted Message:\n" + decryptedText);
		        System.out.println("Sum of ASCII values: " + maxSum);
		    }
}

