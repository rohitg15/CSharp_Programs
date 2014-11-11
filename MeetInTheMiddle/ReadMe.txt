THIS IS THE README FILE FOR THE MEET IN THE MIDDLE EXPLOIT

The Technique:
	Meet in the Middle is an attack performed on double encrypted block ciphers like DES or AES in the ECB mode.
	This attack can be used to compute the two keys used for encryption/decryption in finite time by exploiting an inherent
	flaw in the design of double encrypted block ciphers. 
	
	(ASSUMPTION : This technique is a time memory tradeoff and assumes that we have enough memory to store the intermediate
		results)
	
	1)We bruteforce the keyspace (32 bytes) and store them in memory.
	2) Now we Encrypt the given plaintext using all posible keys and store that in memory as set1
	3) we decrypt the stage 2 cipher text (obtained after double encryption) using all possible keys snd store in memory as set2
	4) Now we compare the 2 sets obtained 
	5) wherever a match occurs, we know that the corresponding keys used to encrypt / decrypt that pair are the two keys used
	
	For the sake of brevity, this demo assumes knowledge of the first 28 bytes of the key and computes the remaining 4 bytes.
	But the same logic can beextended to compute all 32 bytes given enough memory
	
	