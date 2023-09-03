import matplotlib.pyplot as plt

import macrobond_data_api as api

s = api.get_one_series("ustrad2120")

df = s.values_to_pd_data_frame()

plt.plot(df["date"], df["value"])
plt.ylabel(s.metadata["DisplayUnit"])

plt
