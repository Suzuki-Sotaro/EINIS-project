# README.md

## Methodology Details

Our program is engineered to receive two C# source code files as input, and in return, it generates a similarity score founded on a multitude of metrics. This process is a composite of four primary steps: Compilation, Decompilation to IL, Cleansing, and Comparison.

### Compilation and Decompilation

Initially, the source code files provided as input are compiled into binaries. These binaries subsequently undergo decompilation to obtain their IL (Intermediate Language) representation. This phase neutralizes any existing stylistic differences in the code, ensuring the preservation of structural and logical similarities alone.

### Cleansing

Following the decompilation, the IL code is 'cleansed'. This step filters out irrelevant information and emphasizes the vital components of the code structure. To be precise, only the lines containing 'IL_' are retained, while all extraneous information is eliminated, leaving solely the IL instruction set.

### Comparison

Eventually, the cleansed IL code from both files undergoes comparison through several string similarity metrics. These include Jaro-Winkler, Normalized Levenshtein, Cosine similarity, Jaccard index, and Sorensen Dice. Each metric offers a unique perspective on the similarity of the two code files. An average of these metrics is subsequently computed to furnish a comprehensive similarity score.

### Results and Discussion

Our program generates a console report, delivering the calculated similarity percentages for each metric, along with the average percentage. By utilizing IL code and various similarity metrics, our program establishes an effective and fair comparison, capable of detecting sophisticated plagiarism attempts.

## Conclusion

Our program proposes an efficient method for detecting programming plagiarism by analyzing the IL representation of C# code and deploying various string similarity metrics. Future endeavors may concentrate on extending this methodology to other programming languages and enhancing efficiency.