#include <fstream>
#include "graf.h"
ifstream input("graf.txt");
/* se genereaza doua fisiere:
    `circle.svg` si `random.svg`: */
ofstream outsvg1("circle.svg"),outsvg2("random.svg");
Graf gr;
int main()
{
    gr.read(input);
    gr.applyLayout(lcircle);
    gr.save_svg(outsvg1);
    gr.applyLayout(lrandom);
    gr.save_svg(outsvg2);
    return 0;
}
