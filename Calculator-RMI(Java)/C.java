package rmi_srv;
import static java.lang.Math.sqrt;
import java.rmi.*; 
import java.rmi.server.*;

public class C extends UnicastRemoteObject implements I {

    int val;
    public C(int i) throws RemoteException { 
        val = i; 
    }

    public void adunare(int i) { 
        val += i;
        System.out.println("Un client a pus " + i + ". Curent = " + val);
    }

    public void scadere(int i) {
        val -= i;
    }
    
    public void inmultire(int i) {
        val *=i ;
    }
    
    public void impartire(int i) {
        val /= i;
    }
    
    public void inversare() {
        int c=val,invers=0;
        while(c!=0) {
            invers=invers*10+c%10;
            c=c/10;
        }
        val=c;
    }
    
    public void putere(int i) {
        int c=val;
        for(int j=0;j<=1;j++)
            val=val*c;
    }
    
    public void factorial() {
        int c=1;
        for(int i=1;i<=val;i++)
            c=c*i;
        val=c;
    }
    
    public void radical() {
        val=(int)sqrt(val);
    }
    
    public int curent() { 
        return val; 
    }
}

