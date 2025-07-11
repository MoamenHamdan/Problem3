package Sowlutions;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.util.*;

public class Problem2 {
	//Node that storing data
	static class Record {
		String suit, animal, fruit;
		boolean result;

		Record(String suit, String animal, String fruit, String result) {
			this.suit = suit.trim();
			this.animal = animal.trim();
			this.fruit = fruit.trim();
			this.result = result.trim().equalsIgnoreCase("TRUE");
		}
	}

	public static void main(String[] args) {
		String filePath = "C:\\Users\\Admin\\Desktop\\prediction.csv";
		
		List<Record> records = readCsv(filePath);

		if (records.isEmpty()) {
			System.out.println("No data available.");
			return;
		}
		//User Input
		Scanner scanner = new Scanner(System.in);
		System.out.print("Enter Card Suit: ");
		String suit = scanner.nextLine();

		System.out.print("Enter Animal: ");
		String animal = scanner.nextLine();

		System.out.print("Enter Fruit: ");
		String fruit = scanner.nextLine();
		
		//calculating the Probability for each Feature
		double suitProb = calculateFeatureProbability(records, "suit", suit);
		double animalProb = calculateFeatureProbability(records, "animal", animal);
		double fruitProb = calculateFeatureProbability(records, "fruit", fruit);

		double finalProb = (suitProb + animalProb + fruitProb) / 3.0;

		System.out.print("Estimated Win Probability: " + finalProb + "%");
	}

	private static double calculateFeatureProbability(List<Record> records, String feature, String value) {
		long total = 0;
		long wins = 0;
		//iterate on all the records 
		for (Record r : records) {
			boolean matches = switch (feature) {
			case "suit" -> r.suit.equalsIgnoreCase(value);
			case "animal" -> r.animal.equalsIgnoreCase(value);
			case "fruit" -> r.fruit.equalsIgnoreCase(value);
			default -> false;
			};

			if (matches) {
				total++;
				if (r.result)
					wins++;
			}
		}
		//mid-case 
		if (total == 0) {
			return 50.0;
		}
		return (double) wins / total * 100;
	}
	//method that read the data from the excel sheet
	public static List<Record> readCsv(String filePath) {
		List<Record> list = new ArrayList<>();
		try (BufferedReader br = new BufferedReader(new FileReader(filePath))) {
			String line;
			boolean isFirst = true;

			while ((line = br.readLine()) != null) {
				if (isFirst) {
					isFirst = false;
					continue;
				}

				String[] parts = line.split(",");
				if (parts.length >= 4) {
					list.add(new Record(parts[0], parts[1], parts[2], parts[3]));
				}
			}
		} catch (IOException e) {
			System.err.println("Error reading CSV: " + e.getMessage());
		}
		return list;
	}
}
