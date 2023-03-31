#include <iostream>
#include <string>

// TODO: Json for C++
class Observation
{
};

class Action
{
};

Action get_action(Observation observation)
{
    return Action();
}

int main()
{
    while(true) {
        std::string observation_str, action_str;
        std::getline(std::cin, observation_str);

        // action = get_action(observation)

        std::cout << action_str << std::endl;
        std::cout.flush();
    }
}