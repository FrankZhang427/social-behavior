# social-behavior

This is an Unity simulator that simulates the flow of travellers with presence of wanderers, social agents and obstacles.
The scene consists of a large rectangle area and several obstacles (dynamically generated).
A travelling agent spawns at the right side of rectangle area and tries to reach one of the doorway on the left side.
Wanderer agents simply wander around the level with varying but relatively fast speed.
Wanderers try to interfere the travelling agents from reaching their goals.
Social agents move randomly around the level.
When they are close to other social agents or a group of social agents,
they randomly decide whether or not to start a conversation with other social agents or join the social group.
This simulator tries to measure the throughput of travelling agents with interference from wanderers, social agents, and obstacles.
The instruction of setting simulation parameters and test results can be found [here](https://frankzhang427.github.io/pdf/setup_result.pdf).
More detailed simulator design can be found [here](https://frankzhang427.github.io/pdf/social_behavior.pdf).
