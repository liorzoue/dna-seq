# dna-seq

## Licence 

![cc](http://mirrors.creativecommons.org/presskit/icons/cc.svg) 
![by](http://mirrors.creativecommons.org/presskit/icons/by.svg)
![nc](http://mirrors.creativecommons.org/presskit/icons/nc.svg) 
![sa](http://mirrors.creativecommons.org/presskit/icons/sa.svg) 


## Documentation 

Incoming ...

* Loading one file :
	- `console-dna.exe -f INDIVIDU.sam` 
	- `console-dna.exe --file INDIVIDU.sam`

* Loading entire folder (no subfolder) :
	- `console-dna.exe -d D:\data\my-data-to-import` 
	- `console-dna.exe --folder D:\data\my-data-to-import`

* Saving amorce file :
	- `console-dna.exe -a amorces.csv [--truncate]` 
	- `console-dna.exe --amorces amorces.csv [--truncate]`

* Export individu (for debug) :
	- `console-dna.exe -i INDIVIDU` 
	- `console-dna.exe --individu INDIVIDU`

* Export individu (FORMAT = `fasta`) :
	- `console-dna.exe -e FORMAT INDIVIDU` 
	- `console-dna.exe --export FORMAT INDIVIDU`