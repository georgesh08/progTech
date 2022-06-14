package tech.is.service.database;

import tech.is.service.entities.Cat;
import java.util.ArrayList;

// Database

public class CatsDb {
    public static final ArrayList<Cat> Cats = new ArrayList<>();

    public static void AddCat(Cat cat){
        Cats.add(cat);
    }

    public static Cat GetCatById(int id){
        for(Cat cat : Cats){
            if(cat.Id == id){
                return cat;
            }
        }
        return null;
    }
}
