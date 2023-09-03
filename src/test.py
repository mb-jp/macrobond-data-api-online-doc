import matplotlib.pyplot as plt

# from macrobond_data_api.web import WebClient
from macrobond_data_api.com import ComClient

with ComClient() as test:
    pass

import macrobond_data_api as api

# with WebClient() as api:
# series = api.get_one_series("usgdp")
# series.name
# series.values_to_pd_data_frame()

s = api.get_one_series("ustrad2120")

df = s.values_to_pd_data_frame()

plt.plot(df["date"], df["value"])
plt.ylabel(s.metadata["DisplayUnit"])

plt
