## Excersie 3.06 Analysis
### DBaaS vs DIY DB

DBaaS Pros  
  - Fully managed 
  - High Availability

DBaaS Cons:
  - High Cost: ~$29 p/m for low tier
  - OOT for scope of the Todo project. 

DIY Pros:
  - Cost effective, ~$5 p/m for same storage space in Google Persistent Disk.  
  - Automatic data redundancy, snapshots, encryption.

DIY Cons:
  - Additional workload in managing and maintaining. 
  - Within scope of such a small project. 


### Thoughts:
  Even though previous to this exercise I had already moved it to a DIY instance to fix a deployment issue, the scope of the project is much too small for a fully managed database service. 
  On a larger scale, I would still be reluctant to use fully managed services, as as they scale they become very expensive, and to be locked into such a cost  while you're business starts to grow could be detrimental.



## Exercise 3.10 Logging

![image](https://github.com/user-attachments/assets/239ecdc7-b019-4cea-b3a6-2c6c8b4ea8c5)

## Exercise 4.02 Prometheus Query:

`sum(kube_pod_info{namespace="prometheus", created_by_kind="StatefulSet"})`