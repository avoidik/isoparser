# What is it

This is my first program written with MonoDevelop under Linux for crowdin translation project.
It can translate **ISO-3166-2** regions and countries presented in en_US to ru_RU (or you language) with a help of **Wikipedia**.

# What it do

* parses yaml unloaded from crowdin
* builds a tree of regions and countries to translate from yaml
* grabs translation from wikipedia and fill tree with translations
* output translated tree to a file ready for import to crowdin

It will do additional checks for:
* orphaned items in a tree while operations (untranslated will be logged)
* parsed web page and http status codes

# Input yml data

`country.RU: Russia`
`region.RU-AMU: Amurskaya oblast'`

# Output yml data

`country.RU: Россия`
`region.RU-AMU: Амурская область`

# How to adapt to my language

Just change links to wikipedia and recompile.

