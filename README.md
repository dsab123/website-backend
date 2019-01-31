# website-backend-blogposthandler
Lambda function handler for website rewrite.

The frontend of this project can be found [here](https://github.com/dsab123/website).

The most interesting thing about this backend is that it does not hit a database. Well, I guess _technically_ it does. The database is a simple text file!

There are two reasons for this seemingly foolish decision:
- Developers at my job are either _database_ developers or _application_ developers. As an application developer, I am not as experienced as I'd like to be in my understanding of relational databases.
- I've been trolling a database developer at work that a well-formed text file is all you need for a good database _(So Tyler, if you ever read this, I'm biting down on my text file soapbox)_.

The text file database I've titled a ```tagfile```, since it encapsulates the relationships among ```Tags``` of ```BlogPost``` objects.

here's a sample ```tagfile```:

```
1-fun,theology,scripture
2-test,food,scripture
3-bunny,paul,fun
```

the number is the ```BlogPost```.```Id```, and the comma-separated string list are the ```Tags``` of the ```BlogPost``` with the aforementioned ```Id```.

Every time the lambda function is run, it reads this ```tagfile``` and parses it into a ```Dictionary<string, SortedSet<string>>```. Based on the requested ```Id```, it then returns a ```BlogPost``` populated with ```RelatedPosts``` properties that contain one or more ```Tags``` in common with the requested ```Id```.

---------

There's more I could write about this project, but for now ☝️will have to suffice. Once the site is fully-MVP and presentable, I'll write more as a ```BlogPost```, ha!
