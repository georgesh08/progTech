package tech.is.service.database;

import tech.is.service.entities.Owner;
import java.util.ArrayList;

// Database

public class OwnersDb {
    private static final ArrayList<Owner> Owners = new ArrayList<>();

    public static Owner GetById(int id){
        for(Owner owner : Owners){
            if(owner.Id == id){
                return owner;
            }
        }
        return null;
    }

    public static void AddOwner(Owner owner){
        Owners.add(owner);
    }

    public static void AddCatToOwner(int catId, int owner){
        GetById(owner).Cats.add(catId);
    }

    public static boolean IsExisting(int id){
        return GetById(id) != null;
    }
}
