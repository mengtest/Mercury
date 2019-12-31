#include<cstdio>
#include<cmath>
struct obj{
    float x;
    float y;
};
float GetDistanceBetTowObj(obj argumment0, obj argument1);
{
    float y = argument1.y - argument0.y;
    float x = argument1.x - argument0.x;
    return sqrt(y*y + x*x);
}
float GetDirectionBetTowObj(obj argument0, obj argument1)
{
    
    return acos(GetDistanceBetTowObj(argument0,argument1)/(argument1.x - argument0.x)); 
}
float GetDistanceOneObj(obj argument0)
{
    return sqrt(argument0.y * argument0.y + argument0.x * argument0.x);
}
float GetDirectionOneObj(obj argument0)
{
    return acos(GetDirectionOneObj(argument0)/argument0.x);
}
float GetXInPolarCoor(obj argument0, float Distance, float Direction)
{
    return argument0.x + (Distance * cos(Direction));
}
float GetYInPolarCoor(obj argument0, float Distance, float Direction)
{
    return argument0.y + (Distance * sin(Direction));
}
int main ()
{
    return 0;
}