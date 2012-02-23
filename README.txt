
I missed 2 days of work the last weekend so there were 2 features and 1 bug I was not able to implement.

Each pusher is assigned a plan object.  The plan object handles the logic for the markers lifetime.  For example a plan might be to switch between moving a red marker to a vertex and a grey marker to be converted.  The plan object will then make use of goal objects.  The goal objects are short term actions that can be solved.  For example move a red marker to a vertext.

The main program performs the following steps:
  
   - Reads the map
   - Checks each pushers current goal to see if it has finished, if not get the next goal from the plan.
   - Get the next move from the goal.

The idea was to be able to write new plans and goals and switch easily between them to test out strategies and tweak different kinds of actions.

The only other feature of interest was the RegionMap object.  This 100x100 array had a list of all regions a given point on the map "touched".  In this way it was easy to see where a marker was located.

Please post or email me questions if there is interest and I will explain anything else that is not clear.

Thanks again to everyone involved -- this was a lot of fun.