package rmi_cln;
import java.rmi.*; 
import java.util.*;
import rmi_srv.I;

class Menu {
    
    public void menu() {
        System.out.println("      Calculator      ");
        System.out.println();
        System.out.println("1. Adunare");
        System.out.println("2. Scadere");
        System.out.println("3. Inmultire");
        System.out.println("4. Impartire");
        System.out.println("5. Inversare");
        System.out.println("6. Ridicare la putere");
        System.out.println("7. Factorial");
        System.out.println("8. Radical");
        System.out.println("0. Exit");
        System.out.print("\n     Introduceti optiunea dorita: ");
        /*
        Scanner sc=new Scanner(System.in);
        do{
            op=sc.nextInt();
            if(op<0||op>8)
            System.out.print("\n     INTRODUCETI O OPTIUNE VALIDA!\n     Introduceti optiunea dorita: ");
	}
        while(op<0||op>9);*/
    } 
}

public class ClientRMI {

    public static void main(String[] sir) throws RemoteException {
        
        double val; 
        Scanner sc = new Scanner(System.in);
        System.out.print("Adresa server si port: ");
        String adresa = sc.next(); 
        int port = sc.nextInt();
        String url =  "rmi://" + adresa + ":" + port + "/Ob";
        I ref_Ob = null;
        
        try { 
            ref_Ob = (I) Naming.lookup(url); 
        }
        catch(Exception e) {
            System.out.println("Conectare esuata");
            System.exit(0);
        }
        Menu m=new Menu();
        int op;
        for ( ; ; ) {
            m.menu();
            op=sc.nextInt();
            System.out.print("Valoare : ");
            val = sc.nextDouble(); 
            switch(op) {
                case 1:ref_Ob.adunare(val);
                       break;
                case 2:ref_Ob.scadere(val);
                       break;
                case 3:ref_Ob.inmultire(val);
                       break;
                case 4:ref_Ob.impartire(val);
                       break;
                case 5:ref_Ob.inversare();
                       break;
                case 6:ref_Ob.putere(val);
                       break;
                case 7:ref_Ob.factorial();
                       break;
                case 8:ref_Ob.radical();
                       break;
            }
            if (val==0) 
                break;
            System.out.println("Val. curenta este : " + ref_Ob.curent() );
        }
    }
}
