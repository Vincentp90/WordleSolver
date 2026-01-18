from wordfreq import zipf_frequency, top_n_list

print()
words = top_n_list("de", 900000)

all_five_letter_words = [
    w for w in words
    if len(w) == 5 and zipf_frequency(w, "de") > 0
]

with open("all5letter.txt", "w", encoding="utf-8") as f:
    for w in all_five_letter_words:
        f.write(w + "\n")

print("Number of all 5 letter words:")
print(str(len(all_five_letter_words)))

common_five_letter_words = [
    w for w in words
    if len(w) == 5 and zipf_frequency(w, "de") > 3
]

with open("common5letter.txt", "w", encoding="utf-8") as f:
    for w in common_five_letter_words:
        f.write(w + "\n")

print("Number of common 5 letter words:")
print(str(len(common_five_letter_words)))